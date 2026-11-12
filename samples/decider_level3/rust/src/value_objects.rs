use crate::models::*;
#[derive(Clone,Debug,PartialEq,Eq)]
pub struct StudentId(String);
impl StudentId {pub fn from_event(value:String)->Self{Self(value)} pub fn value(&self)->&String{&self.0}}
impl TryFrom<String> for StudentId {type Error=Rejection;fn try_from(v:String)->Result<Self,Rejection>{if v.trim().is_empty(){Err(Rejection::InvalidId)}else{Ok(Self(v))}}}
#[derive(Clone,Debug,PartialEq,Eq)]
pub struct ClassId(String);
impl ClassId {pub fn from_event(value:String)->Self{Self(value)} pub fn value(&self)->&String{&self.0}}
impl TryFrom<String> for ClassId {type Error=Rejection;fn try_from(v:String)->Result<Self,Rejection>{if v.trim().is_empty(){Err(Rejection::InvalidId)}else{Ok(Self(v))}}}
#[derive(Clone,Debug,PartialEq,Eq)]
pub struct SeatCapacity(i32);
impl SeatCapacity {pub fn from_event(value:i32)->Self{Self(value)} pub fn value(&self)->&i32{&self.0}}
impl TryFrom<i32> for SeatCapacity {type Error=Rejection;fn try_from(v:i32)->Result<Self,Rejection>{if (1..=10).contains(&v){Ok(Self(v))}else{Err(Rejection::InvalidCapacity)}}}
#[derive(Clone,Debug,PartialEq,Eq)]
pub struct AssessmentScore(i32);
impl AssessmentScore {pub fn from_event(value:i32)->Self{Self(value)} pub fn value(&self)->&i32{&self.0}}
impl TryFrom<i32> for AssessmentScore {type Error=Rejection;fn try_from(v:i32)->Result<Self,Rejection>{if (0..=100).contains(&v){Ok(Self(v))}else{Err(Rejection::InvalidScore)}}}
#[derive(Clone,Debug,PartialEq,Eq)]
pub struct SchoolDate(String);
impl SchoolDate {pub fn from_event(value:String)->Self{Self(value)} pub fn value(&self)->&String{&self.0}}
fn valid_date(s:&str)->bool{
    let bytes=s.as_bytes();
    if bytes.len()!=10 || bytes[4]!=b'-' || bytes[7]!=b'-' || !bytes.iter().enumerate().all(|(i,c)|i==4 || i==7 || c.is_ascii_digit()){return false;}
    let y=s[0..4].parse::<u32>().unwrap();let m=s[5..7].parse::<usize>().unwrap();let d=s[8..10].parse::<u32>().unwrap();
    let leap=y%4==0 && (y%100!=0 || y%400==0);
    y>0 && (1..=12).contains(&m) && d>0 && d<=[31,if leap{29}else{28},31,30,31,30,31,31,30,31,30,31][m-1]
}

impl TryFrom<String> for SchoolDate {type Error=Rejection;fn try_from(v:String)->Result<Self,Rejection>{if valid_date(&v){Ok(Self(v))}else{Err(Rejection::InvalidDate)}}}
pub struct ValidatedCommand(Command);
impl ValidatedCommand {
    pub fn value(&self)->&Command{&self.0}
    pub fn create(command:Command)->Result<Self,Rejection>{
        match &command{
            Command::RegisterStudent{student_id,..}|Command::PromoteStudent{student_id,..}=>{StudentId::try_from(student_id.clone())?;},
            Command::OpenClass{class_id,capacity,..}=>{ClassId::try_from(class_id.clone())?;SeatCapacity::try_from(*capacity)?;},
            Command::StartClass{class_id,started_on}=>{ClassId::try_from(class_id.clone())?;SchoolDate::try_from(started_on.clone())?;},
            Command::Assess{class_id,student_id,score}=>{ClassId::try_from(class_id.clone())?;StudentId::try_from(student_id.clone())?;AssessmentScore::try_from(*score)?;},
            Command::Enroll{class_id,student_id}|Command::Drop{class_id,student_id}|Command::Attend{class_id,student_id}=>{ClassId::try_from(class_id.clone())?;StudentId::try_from(student_id.clone())?;},
            Command::CompleteClass{class_id}=>{ClassId::try_from(class_id.clone())?;},
        }
        Ok(Self(command))
    }
}
