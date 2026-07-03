using R3;

namespace Game
{
    public interface IBaseModel
    {
        ReactiveProperty<int> Health { get; }

        int MaxHealth { get; }

        void ApplyDamage(int damage);
    }
}