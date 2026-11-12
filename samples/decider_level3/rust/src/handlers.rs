use crate::{dcb::*,models::*,routing,decider,projection,value_objects::ValidatedCommand};
pub struct SchoolHandler;
impl CommandHandler<Command,Event,Rejection> for SchoolHandler {
    fn validate(&self,c:&Command)->Vec<Rejection>{ValidatedCommand::create(c.clone()).err().into_iter().collect()}
    fn query_for(&self,c:&Command)->Query{routing::query_for(c)}
    fn handle(&self,events:&[Recorded<Event>],command:&Command)->Preparation<Event,Rejection>{
        match ValidatedCommand::create(command.clone()){
            Err(error)=>Preparation::Reject(error),
            Ok(validated)=>match decider::decide(&projection::decision_model(&projection::fold(events)),&validated){
                Decision::Accepted{events}=>Preparation::Emit(events),Decision::Rejected{code}=>Preparation::Reject(code)
            }
        }
    }
}
