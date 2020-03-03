using System.Threading.Tasks;

namespace DEVICE_CORE.StateMachine.State
{
    public interface IStateControlTrigger<TStateAction>
    {
        Task Complete(TStateAction state);
        Task Error(TStateAction state);
    }
}
