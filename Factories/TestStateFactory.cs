using RayLibECS.EntityStates;
using RayLibECS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayLibECS.Factories
{
    internal class TestStateFactory : IStateFactory
    {
        private Dictionary<string, EntityStateBase> _cachedStates;

        internal TestStateFactory()
        {
            _cachedStates = new Dictionary<string, EntityStateBase>();
        }
        public EntityStateBase CreateState(string state)
        {
            switch (state)
            {
                case "test":
                    return _cachedStates.ContainsKey("test")
                        ? _cachedStates["test"]
                        : NewState("test", new EntityTestState());

            }
            throw new Exception("invalid state");
        }

        public EntityStateBase NewState(string key, EntityStateBase stateBase)
        {
            _cachedStates.Add(key, stateBase);
            return _cachedStates[key];
        }
    }
}
