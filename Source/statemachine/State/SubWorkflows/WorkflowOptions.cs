using XO.Requests;

namespace StateMachine.State.SubWorkflows
{
    internal class WorkflowOptions
    {
        public int? ExecutionTimeout;
        public LinkRequest StateObject;
    }
}
