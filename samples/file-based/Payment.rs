// Run: python3 scripts/run_payment_rust.py
// 安定版Cargo用の一時manifestはrunnerが作成。サンプル本体はこの1ファイル。
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
#[serde(tag = "$type", rename_all = "snake_case", rename_all_fields = "camelCase")]
enum Payment {
    Cash { amount: u64, next: Option<Box<Payment>> },
    Card { card_number: String, amount: u64, next: Option<Box<Payment>> },
}

// No wildcard: adding a variant or omitting an arm is a compiler error (E0004).
fn describe(payment: &Payment) -> String {
    match payment {
        Payment::Cash { amount, .. } => format!("cash:{amount}"),
        Payment::Card { card_number, .. } => format!("card:{card_number}"),
    }
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let original = Payment::Cash {
        amount: 100,
        next: Some(Box::new(Payment::Card {
            card_number: "4111".into(), amount: 250, next: None,
        })),
    };
    let json = serde_json::to_string(&original)?;
    let restored: Payment = serde_json::from_str(&json)?;
    println!("{json}");
    println!("{}", describe(&restored));
    Ok(())
}
