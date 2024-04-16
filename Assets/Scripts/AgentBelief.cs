using System;
using System.Collections.Generic;
using UnityEngine;
namespace GOAP
{
    public class BeliefFactory
    {
        private readonly GoapAgent _agent;
        private readonly Dictionary<string, AgentBelief> _beliefs;

        public BeliefFactory(GoapAgent agent, Dictionary<string,AgentBelief> beliefs)
        {
            this._agent = agent;
            this._beliefs = beliefs;
        }

        public void AddBelief(string key, Func<bool> condition)
        {
            _beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(condition)
                .Build());
        }

        public void AddLocationBelief(string key, float distance, Transform locationCondition)
        {
            AddLocationBelief(key,distance,locationCondition.position);
        }
        
        public void AddLocationBelief(string key, float distance, Vector3 locationCondition)
        {
            _beliefs.Add(key,new AgentBelief.Builder(key)
                .WithCondition(() => IsRangeOf(locationCondition,distance))
                .WithLocation(() => locationCondition)
                .Build());
        }
        
        //TODO ADd Senser Belief

        bool IsRangeOf(Vector3 pos, float range) => Vector3.Distance(_agent.transform.position, pos) < range;
    }
    
    public class AgentBelief
    {
        public string Name { get; }

        private Func<bool> condition = () => false;
        private Func<Vector3> observedLocation = () => Vector3.zero;

        public Vector3 Location => observedLocation();

        public AgentBelief(string name)
        {
            Name = name;
        }

        public bool Ecaluate() => condition();
        
        public class  Builder
        {
            private readonly AgentBelief _belief;

            public Builder(string name)
            {
                _belief = new AgentBelief(name);
            }

            public Builder WithCondition(Func<bool> condition)
            {
                _belief.condition = condition;
                return this;
            }

            public Builder WithLocation(Func<Vector3> observedLocation)
            {
                _belief.observedLocation = observedLocation;
                return this;
            }

            public AgentBelief Build()
            {
                return _belief;
            }
        }
    }
    
}
