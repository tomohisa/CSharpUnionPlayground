use serde::{Serialize, Deserialize};
use std::{collections::BTreeMap, time::Instant, hint::black_box};
#[derive(Serialize, Deserialize)]
struct Payload { id: i32, label: String, values: Vec<i32>, tags: BTreeMap<String,String>, next: Option<Box<Payment>> }
#[derive(Serialize, Deserialize)]
struct Case000 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case001 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case002 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case003 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case004 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case005 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case006 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case007 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case008 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case009 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case010 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case011 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case012 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case013 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case014 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case015 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case016 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case017 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case018 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case019 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case020 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case021 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case022 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case023 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case024 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case025 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case026 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case027 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case028 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case029 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case030 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case031 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case032 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case033 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case034 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case035 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case036 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case037 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case038 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case039 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case040 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case041 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case042 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case043 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case044 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case045 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case046 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case047 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case048 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case049 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case050 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case051 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case052 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case053 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case054 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case055 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case056 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case057 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case058 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case059 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case060 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case061 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case062 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case063 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case064 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case065 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case066 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case067 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case068 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case069 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case070 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case071 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case072 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case073 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case074 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case075 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case076 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case077 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case078 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case079 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case080 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case081 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case082 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case083 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case084 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case085 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case086 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case087 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case088 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case089 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case090 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case091 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case092 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case093 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case094 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case095 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case096 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case097 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case098 { data: Payload }
#[derive(Serialize, Deserialize)]
struct Case099 { data: Payload }
#[derive(Serialize, Deserialize)]
#[serde(tag="$type")]
enum Payment {
    Case000(Case000),
    Case001(Case001),
    Case002(Case002),
    Case003(Case003),
    Case004(Case004),
    Case005(Case005),
    Case006(Case006),
    Case007(Case007),
    Case008(Case008),
    Case009(Case009),
    Case010(Case010),
    Case011(Case011),
    Case012(Case012),
    Case013(Case013),
    Case014(Case014),
    Case015(Case015),
    Case016(Case016),
    Case017(Case017),
    Case018(Case018),
    Case019(Case019),
    Case020(Case020),
    Case021(Case021),
    Case022(Case022),
    Case023(Case023),
    Case024(Case024),
    Case025(Case025),
    Case026(Case026),
    Case027(Case027),
    Case028(Case028),
    Case029(Case029),
    Case030(Case030),
    Case031(Case031),
    Case032(Case032),
    Case033(Case033),
    Case034(Case034),
    Case035(Case035),
    Case036(Case036),
    Case037(Case037),
    Case038(Case038),
    Case039(Case039),
    Case040(Case040),
    Case041(Case041),
    Case042(Case042),
    Case043(Case043),
    Case044(Case044),
    Case045(Case045),
    Case046(Case046),
    Case047(Case047),
    Case048(Case048),
    Case049(Case049),
    Case050(Case050),
    Case051(Case051),
    Case052(Case052),
    Case053(Case053),
    Case054(Case054),
    Case055(Case055),
    Case056(Case056),
    Case057(Case057),
    Case058(Case058),
    Case059(Case059),
    Case060(Case060),
    Case061(Case061),
    Case062(Case062),
    Case063(Case063),
    Case064(Case064),
    Case065(Case065),
    Case066(Case066),
    Case067(Case067),
    Case068(Case068),
    Case069(Case069),
    Case070(Case070),
    Case071(Case071),
    Case072(Case072),
    Case073(Case073),
    Case074(Case074),
    Case075(Case075),
    Case076(Case076),
    Case077(Case077),
    Case078(Case078),
    Case079(Case079),
    Case080(Case080),
    Case081(Case081),
    Case082(Case082),
    Case083(Case083),
    Case084(Case084),
    Case085(Case085),
    Case086(Case086),
    Case087(Case087),
    Case088(Case088),
    Case089(Case089),
    Case090(Case090),
    Case091(Case091),
    Case092(Case092),
    Case093(Case093),
    Case094(Case094),
    Case095(Case095),
    Case096(Case096),
    Case097(Case097),
    Case098(Case098),
    Case099(Case099),
}
fn create(seed: i32, depth: i32) -> Payment {
    let data = Payload { id: seed, label: format!("item-{seed}"), values: vec![seed,seed+1,seed+2,seed+3], tags: BTreeMap::from([("group".into(),format!("g{}",seed%7)),("note".into(),"bench".into())]), next: if depth>1 { Some(Box::new(create(seed+37,depth-1))) } else {None} };
    match seed%100 {
        0 => Payment::Case000(Case000 { data }),
        1 => Payment::Case001(Case001 { data }),
        2 => Payment::Case002(Case002 { data }),
        3 => Payment::Case003(Case003 { data }),
        4 => Payment::Case004(Case004 { data }),
        5 => Payment::Case005(Case005 { data }),
        6 => Payment::Case006(Case006 { data }),
        7 => Payment::Case007(Case007 { data }),
        8 => Payment::Case008(Case008 { data }),
        9 => Payment::Case009(Case009 { data }),
        10 => Payment::Case010(Case010 { data }),
        11 => Payment::Case011(Case011 { data }),
        12 => Payment::Case012(Case012 { data }),
        13 => Payment::Case013(Case013 { data }),
        14 => Payment::Case014(Case014 { data }),
        15 => Payment::Case015(Case015 { data }),
        16 => Payment::Case016(Case016 { data }),
        17 => Payment::Case017(Case017 { data }),
        18 => Payment::Case018(Case018 { data }),
        19 => Payment::Case019(Case019 { data }),
        20 => Payment::Case020(Case020 { data }),
        21 => Payment::Case021(Case021 { data }),
        22 => Payment::Case022(Case022 { data }),
        23 => Payment::Case023(Case023 { data }),
        24 => Payment::Case024(Case024 { data }),
        25 => Payment::Case025(Case025 { data }),
        26 => Payment::Case026(Case026 { data }),
        27 => Payment::Case027(Case027 { data }),
        28 => Payment::Case028(Case028 { data }),
        29 => Payment::Case029(Case029 { data }),
        30 => Payment::Case030(Case030 { data }),
        31 => Payment::Case031(Case031 { data }),
        32 => Payment::Case032(Case032 { data }),
        33 => Payment::Case033(Case033 { data }),
        34 => Payment::Case034(Case034 { data }),
        35 => Payment::Case035(Case035 { data }),
        36 => Payment::Case036(Case036 { data }),
        37 => Payment::Case037(Case037 { data }),
        38 => Payment::Case038(Case038 { data }),
        39 => Payment::Case039(Case039 { data }),
        40 => Payment::Case040(Case040 { data }),
        41 => Payment::Case041(Case041 { data }),
        42 => Payment::Case042(Case042 { data }),
        43 => Payment::Case043(Case043 { data }),
        44 => Payment::Case044(Case044 { data }),
        45 => Payment::Case045(Case045 { data }),
        46 => Payment::Case046(Case046 { data }),
        47 => Payment::Case047(Case047 { data }),
        48 => Payment::Case048(Case048 { data }),
        49 => Payment::Case049(Case049 { data }),
        50 => Payment::Case050(Case050 { data }),
        51 => Payment::Case051(Case051 { data }),
        52 => Payment::Case052(Case052 { data }),
        53 => Payment::Case053(Case053 { data }),
        54 => Payment::Case054(Case054 { data }),
        55 => Payment::Case055(Case055 { data }),
        56 => Payment::Case056(Case056 { data }),
        57 => Payment::Case057(Case057 { data }),
        58 => Payment::Case058(Case058 { data }),
        59 => Payment::Case059(Case059 { data }),
        60 => Payment::Case060(Case060 { data }),
        61 => Payment::Case061(Case061 { data }),
        62 => Payment::Case062(Case062 { data }),
        63 => Payment::Case063(Case063 { data }),
        64 => Payment::Case064(Case064 { data }),
        65 => Payment::Case065(Case065 { data }),
        66 => Payment::Case066(Case066 { data }),
        67 => Payment::Case067(Case067 { data }),
        68 => Payment::Case068(Case068 { data }),
        69 => Payment::Case069(Case069 { data }),
        70 => Payment::Case070(Case070 { data }),
        71 => Payment::Case071(Case071 { data }),
        72 => Payment::Case072(Case072 { data }),
        73 => Payment::Case073(Case073 { data }),
        74 => Payment::Case074(Case074 { data }),
        75 => Payment::Case075(Case075 { data }),
        76 => Payment::Case076(Case076 { data }),
        77 => Payment::Case077(Case077 { data }),
        78 => Payment::Case078(Case078 { data }),
        79 => Payment::Case079(Case079 { data }),
        80 => Payment::Case080(Case080 { data }),
        81 => Payment::Case081(Case081 { data }),
        82 => Payment::Case082(Case082 { data }),
        83 => Payment::Case083(Case083 { data }),
        84 => Payment::Case084(Case084 { data }),
        85 => Payment::Case085(Case085 { data }),
        86 => Payment::Case086(Case086 { data }),
        87 => Payment::Case087(Case087 { data }),
        88 => Payment::Case088(Case088 { data }),
        89 => Payment::Case089(Case089 { data }),
        90 => Payment::Case090(Case090 { data }),
        91 => Payment::Case091(Case091 { data }),
        92 => Payment::Case092(Case092 { data }),
        93 => Payment::Case093(Case093 { data }),
        94 => Payment::Case094(Case094 { data }),
        95 => Payment::Case095(Case095 { data }),
        96 => Payment::Case096(Case096 { data }),
        97 => Payment::Case097(Case097 { data }),
        98 => Payment::Case098(Case098 { data }),
        99 => Payment::Case099(Case099 { data }),
        _=>unreachable!()
    }
}
fn score(payment: &Payment) -> i64 {
    let (data,index) = match payment {
        Payment::Case000(v) => (&v.data, 0),
        Payment::Case001(v) => (&v.data, 1),
        Payment::Case002(v) => (&v.data, 2),
        Payment::Case003(v) => (&v.data, 3),
        Payment::Case004(v) => (&v.data, 4),
        Payment::Case005(v) => (&v.data, 5),
        Payment::Case006(v) => (&v.data, 6),
        Payment::Case007(v) => (&v.data, 7),
        Payment::Case008(v) => (&v.data, 8),
        Payment::Case009(v) => (&v.data, 9),
        Payment::Case010(v) => (&v.data, 10),
        Payment::Case011(v) => (&v.data, 11),
        Payment::Case012(v) => (&v.data, 12),
        Payment::Case013(v) => (&v.data, 13),
        Payment::Case014(v) => (&v.data, 14),
        Payment::Case015(v) => (&v.data, 15),
        Payment::Case016(v) => (&v.data, 16),
        Payment::Case017(v) => (&v.data, 17),
        Payment::Case018(v) => (&v.data, 18),
        Payment::Case019(v) => (&v.data, 19),
        Payment::Case020(v) => (&v.data, 20),
        Payment::Case021(v) => (&v.data, 21),
        Payment::Case022(v) => (&v.data, 22),
        Payment::Case023(v) => (&v.data, 23),
        Payment::Case024(v) => (&v.data, 24),
        Payment::Case025(v) => (&v.data, 25),
        Payment::Case026(v) => (&v.data, 26),
        Payment::Case027(v) => (&v.data, 27),
        Payment::Case028(v) => (&v.data, 28),
        Payment::Case029(v) => (&v.data, 29),
        Payment::Case030(v) => (&v.data, 30),
        Payment::Case031(v) => (&v.data, 31),
        Payment::Case032(v) => (&v.data, 32),
        Payment::Case033(v) => (&v.data, 33),
        Payment::Case034(v) => (&v.data, 34),
        Payment::Case035(v) => (&v.data, 35),
        Payment::Case036(v) => (&v.data, 36),
        Payment::Case037(v) => (&v.data, 37),
        Payment::Case038(v) => (&v.data, 38),
        Payment::Case039(v) => (&v.data, 39),
        Payment::Case040(v) => (&v.data, 40),
        Payment::Case041(v) => (&v.data, 41),
        Payment::Case042(v) => (&v.data, 42),
        Payment::Case043(v) => (&v.data, 43),
        Payment::Case044(v) => (&v.data, 44),
        Payment::Case045(v) => (&v.data, 45),
        Payment::Case046(v) => (&v.data, 46),
        Payment::Case047(v) => (&v.data, 47),
        Payment::Case048(v) => (&v.data, 48),
        Payment::Case049(v) => (&v.data, 49),
        Payment::Case050(v) => (&v.data, 50),
        Payment::Case051(v) => (&v.data, 51),
        Payment::Case052(v) => (&v.data, 52),
        Payment::Case053(v) => (&v.data, 53),
        Payment::Case054(v) => (&v.data, 54),
        Payment::Case055(v) => (&v.data, 55),
        Payment::Case056(v) => (&v.data, 56),
        Payment::Case057(v) => (&v.data, 57),
        Payment::Case058(v) => (&v.data, 58),
        Payment::Case059(v) => (&v.data, 59),
        Payment::Case060(v) => (&v.data, 60),
        Payment::Case061(v) => (&v.data, 61),
        Payment::Case062(v) => (&v.data, 62),
        Payment::Case063(v) => (&v.data, 63),
        Payment::Case064(v) => (&v.data, 64),
        Payment::Case065(v) => (&v.data, 65),
        Payment::Case066(v) => (&v.data, 66),
        Payment::Case067(v) => (&v.data, 67),
        Payment::Case068(v) => (&v.data, 68),
        Payment::Case069(v) => (&v.data, 69),
        Payment::Case070(v) => (&v.data, 70),
        Payment::Case071(v) => (&v.data, 71),
        Payment::Case072(v) => (&v.data, 72),
        Payment::Case073(v) => (&v.data, 73),
        Payment::Case074(v) => (&v.data, 74),
        Payment::Case075(v) => (&v.data, 75),
        Payment::Case076(v) => (&v.data, 76),
        Payment::Case077(v) => (&v.data, 77),
        Payment::Case078(v) => (&v.data, 78),
        Payment::Case079(v) => (&v.data, 79),
        Payment::Case080(v) => (&v.data, 80),
        Payment::Case081(v) => (&v.data, 81),
        Payment::Case082(v) => (&v.data, 82),
        Payment::Case083(v) => (&v.data, 83),
        Payment::Case084(v) => (&v.data, 84),
        Payment::Case085(v) => (&v.data, 85),
        Payment::Case086(v) => (&v.data, 86),
        Payment::Case087(v) => (&v.data, 87),
        Payment::Case088(v) => (&v.data, 88),
        Payment::Case089(v) => (&v.data, 89),
        Payment::Case090(v) => (&v.data, 90),
        Payment::Case091(v) => (&v.data, 91),
        Payment::Case092(v) => (&v.data, 92),
        Payment::Case093(v) => (&v.data, 93),
        Payment::Case094(v) => (&v.data, 94),
        Payment::Case095(v) => (&v.data, 95),
        Payment::Case096(v) => (&v.data, 96),
        Payment::Case097(v) => (&v.data, 97),
        Payment::Case098(v) => (&v.data, 98),
        Payment::Case099(v) => (&v.data, 99),
    };
    index + data.id as i64 + data.label.len() as i64 + data.values.iter().map(|v|*v as i64).sum::<i64>() + data.tags.iter().map(|(k,v)|(k.len()+v.len()) as i64).sum::<i64>() + data.next.as_ref().map_or(0,|p|score(p))
}
fn main() {
    use std::io::Write;
    println!("{{\"event\":\"ready\"}}");std::io::stdout().flush().unwrap();
    let args: Vec<String> = std::env::args().collect();
    let roots: i32 = args[1].parse().unwrap(); let samples: usize = args[2].parse().unwrap();
    let warm: Vec<_> = (0..roots.min(1000)).map(|i|create(i,3)).collect();
    let _: Vec<Payment> = serde_json::from_slice(&serde_json::to_vec(&warm).unwrap()).unwrap();
    let original: Vec<_>=(0..roots).map(|i|create(i,3)).collect();
    let expected: i64=original.iter().map(score).sum();
    let mut measurements=Vec::new(); let mut sink=0i64;
    println!("{{\"event\":\"prepared\"}}");std::io::stdout().flush().unwrap();
    for trial in 0..samples {
        let start=Instant::now();
        let json=serde_json::to_vec(black_box(&original)).unwrap();
        let encoded=start.elapsed().as_secs_f64();
        let copy: Vec<Payment>=serde_json::from_slice(black_box(&json)).unwrap();
        let decoded=start.elapsed().as_secs_f64();
        let sum:i64=black_box(&copy).iter().map(score).sum();
        let total=start.elapsed().as_secs_f64();
        assert_eq!(sum,expected); assert_eq!(copy.len(),roots as usize);
        if trial==0 {assert_eq!(json,serde_json::to_vec(&copy).unwrap());}
        sink+=sum;
        measurements.push(serde_json::json!({"encodeSeconds":encoded,"decodeSeconds":decoded-encoded,"switchSeconds":total-decoded,"totalSeconds":total,"bytes":json.len()}));
    }
    println!("{}",serde_json::json!({"roots":roots,"samples":samples,"sink":sink,"measurements":measurements}));
}
