using System.Threading.Tasks;

namespace StateMachine.State
{
    public interface IStateControlTrigger<TStateAction>
    {
        Task Complete(TStateAction state);
        Task Error(TStateAction state);
    }
}
