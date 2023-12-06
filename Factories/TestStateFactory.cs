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
        private Dictionary<string, IEntityState> _cachedStates;

        internal TestStateFactory()
        {
            _cachedStates = new Dictionary<string, IEntityState>();
        }
        public IEntityState CreateState(string state)
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

        public IEntityState NewState(string key, IEntityState state)
        {
            _cachedStates.Add(key, state);
            return _cachedStates[key];
        }
    }
}
