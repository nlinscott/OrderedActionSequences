using UnityEngine;

namespace OrderedActionSequences.ActionSequences.Base
{
    abstract class SetCharacterPositionBase : MonoBehaviour, IActionSequence
    {
        protected abstract Vector3 GetNewPosition();

        protected abstract Quaternion GetNewRotation();

        [SerializeField]
        private Transform _player;

        public ICompletionSource OnEnd()
        {
            return CompletionSource.Completed;
        }

        public ICompletionSource OnStart()
        {
            _player.SetPositionAndRotation(GetNewPosition(), GetNewRotation());
            return CompletionSource.Completed;
        }
    }
}
