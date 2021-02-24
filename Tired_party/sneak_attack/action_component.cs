using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Tired_party.sneak_attack
{
    class action_component : AgentComponent
    {
        private Agent _agent;
        public Agent agent {  get { return _agent; } }
        public bool first_action_begin = false;
        public bool second_action_begin = false;
        public bool last_action_begin = false;
        public bool last_aciton_end = false;
        private readonly ActionIndexCache first_action = ActionIndexCache.Create("act_dungeon_prisoner_sleep_wakeup");
        private readonly ActionIndexCache second_action = ActionIndexCache.Create("act_scared_idle_" + (MBRandom.RandomInt() % 3).ToString());
        private readonly ActionIndexCache last_action = ActionIndexCache.Create("act_scared_to_normal_" + (MBRandom.RandomInt() % 3).ToString());
        public MissionTimer first_action_checker;
        public MissionTimer second_action_checker;
        public MissionTimer last_action_checker;
       
        public action_component(Agent agent) : base(agent)
        {
            _agent = agent;
        }

        protected override void OnTickAsAI(float dt)
        {
            if(first_action_begin)
            {
                agent.SetActionChannel(0, first_action, false, 0UL, 0f, 3f, -0.2f, 0.4f, MBRandom.RandomFloat * 2 + 1f, false, -0.2f, 0, true);
                first_action_begin = false;
                first_action_checker = new MissionTimer(0.1f);
            }
            if(first_action_checker != null && first_action_checker.Check(true) && agent.GetCurrentActionProgress(0) > 0.9f)
            {
                second_action_begin = true;
                first_action_checker = null;
            }
            if(second_action_begin)
            {
                agent.SetActionChannel(0, first_action, false, 0UL, 0f, 0.6f, -0.2f, 0.4f, MBRandom.RandomFloat * 2, false, -0.2f, 0, true);
                second_action_begin = false;
                second_action_checker = new MissionTimer(0.1f);
            }
            if(second_action_checker != null && second_action_checker.Check(true) && agent.GetCurrentActionProgress(0) > 0.9f)
            {
                last_action_begin = true;
                second_action_checker = null;
            }
            if (last_action_begin)
            {
                agent.SetActionChannel(0, last_action, false, 0UL, 0f, 0.6f, -0.2f, 2f, MBRandom.RandomFloat, true, -0.2f, 0, true);
                last_action_begin = false;
            }
        }
    }
}
