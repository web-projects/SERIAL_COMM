using System.Threading.Tasks;

namespace SERIAL_COMM.StateMachine.State
{
    public interface IStateControlTrigger<TStateAction>
    {
        Task Complete(TStateAction state);
        Task Error(TStateAction state);
    }
}
