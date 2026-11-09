// Run: python3 scripts/run_payment_rust.py
// 安定版Cargo用の一時manifestはrunnerが作成。サンプル本体はこの1ファイル。
use serde::{Deserialize, Serialize};

#[derive(Debug, PartialEq, Serialize, Deserialize)]
#[serde(tag = "$type", rename_all = "snake_case", rename_all_fields = "camelCase")]
enum Payment {
    Cash { amount: u64, next: Option<Box<Payment>> },
    Card { card_number: String, amount: u64, next: Option<Box<Payment>> },
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
    assert_eq!(original, restored);
    // Serdeは内側のケースも検証する。
    assert!(serde_json::from_str::<Payment>(
        r#"{"$type":"cash","amount":100,"next":{"$type":"unknown"}}"#
    ).is_err());
    println!("PASS");
    Ok(())
}
