using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Tired_party.Mission_time;
using Tired_party.Patch;

namespace Tired_party.sneak_attack
{
    class patrol_component : AgentComponent
    {
        private Agent _agent;
        public bool is_finish;
        public Vec2 next_position;
        MissionTimer begin_time;
        int location;
        Vec2 add_num;
        int direction_flag ;
        float x = 0;
        float y = 0;
        int tick_times;
        public Agent agent
        {
            get { return _agent; }
        }
        public patrol_component(Agent agent) : base(agent)
        {
            location = -1;
            _agent = agent;
            is_finish = false;
            direction_flag = MBRandom.RandomFloat > 0.5 ? 1 : -1;
            tick_times = 0;
        }

        protected override void OnTickAsAI(float dt)
        {
            if(!is_finish)
            {
                if(Mission.Current.IsLoadingFinished)
                {
                    go_to_patrol();
                    calculate_ans_position();
                    is_finish = true;
                    begin_time = new MissionTimer(0.3f);
                }
                return;
            }
            if (begin_time.Check(true) && agent.IsActive())
            {
                AgentAIStateFlagComponent ai_flag_component = agent.GetComponent<AgentAIStateFlagComponent>();
                Vec2 agent_position = agent.Position.AsVec2;
                if (ai_flag_component != null)
                {
                    if (ai_flag_component.CurrentWatchState == AgentAIStateFlagComponent.WatchState.Patroling && next_position != null && next_position.Distance(agent_position) < 1)
                    {
                        calculate_ans_position();
                    }
                    else
                    {
                        tick_times++;
                        if(tick_times >= 10)
                        {
                            calculate_ans_position();
                        }
                    }
                }
            }
        }

        private void calculate_ans_position()
        {
            AgentAIStateFlagComponent ai_flag_component = agent.GetComponent<AgentAIStateFlagComponent>();
            Vec2 agent_position = agent.Position.AsVec2;
            if (ai_flag_component != null)
            {
                if (ai_flag_component.CurrentWatchState == AgentAIStateFlagComponent.WatchState.Patroling)
                {
                    Vec2 x_vector = new Vec2(-agent.Formation.Direction.y, agent.Formation.Direction.x);
                    Vec2 y_vector = agent.Formation.Direction;
                    x_vector.Normalize();
                    y_vector.Normalize();
                    Vec2 formation_position = agent.Formation.CurrentPosition;
                    float depth = agent.Formation.Depth / 2;
                    float width = agent.Formation.Width;
                    if(depth < 15f)
                    {
                        depth = 15f;
                    }
                    Vec2 ans_position;
                    switch (location)
                    {
                        case 0:
                            x += direction_flag;
                            y += MBRandom.RandomFloat * 2;
                            if (x < -width || x > width)
                            {
                                direction_flag = -direction_flag;
                                x += direction_flag;
                            }
                            if (y < 5 || y > 30)
                            {
                                y = 17;
                            }
                            ans_position = x_vector * x + y_vector * (depth + y) + formation_position;
                            break;
                        case 1:
                            x += MBRandom.RandomFloat * 2;
                            y += direction_flag;
                            if (x < 5 || x > 15)
                            {
                                x = 10;
                            }
                            if (y < -depth || y > depth)
                            {
                                direction_flag = -direction_flag;
                                y += direction_flag;
                            }
                            ans_position = x_vector * (-width - x) + y_vector * y + formation_position;
                            break;
                        case 2:
                            x += MBRandom.RandomFloat * 2;
                            y += direction_flag;
                            if (x < 5 || x > 15)
                            {
                                x = 10;
                            }
                            if (y < -depth || y > depth)
                            {
                                direction_flag = -direction_flag;
                                y += direction_flag;
                            }
                            ans_position = x_vector * (width + x) + y_vector * y + formation_position;
                            break;
                        case 3:
                            x += direction_flag;
                            y += MBRandom.RandomFloat * 2;
                            if (x < -width || x > width)
                            {
                                direction_flag = -direction_flag;
                                x += direction_flag;
                            }
                            if (y < 5 || y > 30)
                            {
                                y = 17;
                            }
                            ans_position = x_vector * x + y_vector * (-depth - y) + formation_position;
                            break;
                        default:
                            ans_position = formation_position;
                            break;
                    }
                    WorldPosition worldPosition = new WorldPosition(Mission.Current.Scene, new Vec3(ans_position));
                    next_position = worldPosition.AsVec2;
                    agent.SetScriptedPosition(ref worldPosition, false, Agent.AIScriptedFrameFlags.ConsiderRotation | Agent.AIScriptedFrameFlags.DoNotRun);

                }
            }

        }

        private void go_to_patrol()
        {
            float num = MBRandom.RandomFloat;
            if(num <= 0.3)
            {
                location = 0;//上
            }
            else if(num <= 0.5)
            {
                location = 1;//左
            }
            else if(num <= 0.7)
            {
                location = 2;//右
            }
            else if(num <= 0.8)
            {
                location = 3;//下
            }
        }
    }
}
