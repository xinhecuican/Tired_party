using JetBrains.Annotations;
using SandBox.View.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.SaveSystem;
using Tired_party.Information_Screen;

namespace Tired_party
{
    [SaveableClass(34124065)]
    public class Party_tired
    {
        [SaveableField(1)]
        public Dictionary<MobileParty, tired_party_data> _party_tired_rate;
        [SaveableField(2)]
        private static Party_tired _party_tired;
        [SaveableField(3)]
        public List<information_data> information;

        public static float recovery_in_day_time = 0.25f;
        public static float recovery_in_night_time = 0.33f;
        public static float begin_to_decrease = 0.3f;
		public static bool is_sneak_mission = false;
        public static MobileParty test_party = null;
        public Dictionary<MobileParty, tired_party_data> Party_tired_rate
        {
            get
            {
                if(_party_tired_rate == null)
                {
                    _party_tired_rate = new Dictionary<MobileParty, tired_party_data>();
                }
                return _party_tired_rate;
            }
            
            set
            {
                this._party_tired_rate = value;
            }
        }

        public static Party_tired Current
        {
            get
            {
                if(Party_tired._party_tired == null)
                {
                    Party_tired._party_tired = new Party_tired();
                }
                return Party_tired._party_tired;
            }
            set
            {
                Party_tired._party_tired = value;
            }
        }

        public List<information_data> Information { get => information; set => information = value; }

		public static bool is_using()
        {
			return Party_tired.Current != null;
        }

        public static void add_to_dict(MobileParty mobileParty, float now_tired = 1)
        {
            if(mobileParty.IsCaravan || mobileParty.IsVillager)
            {
                return;
            }
            float rate = Calculate_party_tired.calculate_ratio(mobileParty);
            tired_party_data data = new tired_party_data(1.0f, rate, mobileParty.MemberRoster.TotalManCount);
            data.AiBehavior = mobileParty.DefaultBehavior;
            data.ai_behavior_object = mobileParty.TargetSettlement;
            data.ai_behavior_target = mobileParty.TargetPosition;
            data.target_party = mobileParty.TargetParty;
            Current._party_tired_rate.Add(mobileParty, data);
        }

        public Party_tired()
        {
            this._party_tired_rate = new Dictionary<MobileParty, tired_party_data>();
			Information = new List<information_data>();
			Information.Add(new information_data(new TextObject("{=unxsFo8XOr}all information", null)));
			Information.Add(new information_data(new TextObject("{=NLi473iHTH}hidden information", null)));
			Information.Add(new information_data(new TextObject("{=ylstgZWQuD}important information", null)));
        }

		public static void ToggleTent(PartyBase party, bool showTent)
		{
			PartyVisual partyVisuals = (PartyVisual)party.Visuals;
			GameEntity strategicEntity = partyVisuals.StrategicEntity;
			if (showTent)
			{
				bool flag2 = party.MobileParty.CurrentSettlement != null || party.MapEvent != null || party.SiegeEvent != null;
				if (!flag2)
				{
					MatrixFrame matrix = MatrixFrame.Identity;
					matrix.rotation.ApplyScaleLocal(1.2f);
					GameEntity gameEntity = GameEntity.CreateEmpty(strategicEntity.Scene, true);
					gameEntity.Name = "Tent";
					gameEntity.AddMultiMesh(MetaMesh.GetCopy("map_icon_siege_camp_tent", true, false), true);
					gameEntity.SetFrame(ref matrix);
					string text = null;
					CharacterObject leader = party.Leader;
					Hero heroObject = (leader != null) ? leader.HeroObject : null;
					bool flag3 = ((heroObject != null) ? heroObject.ClanBanner : null) != null;
					if (flag3)
					{
						text = party.Leader.HeroObject.ClanBanner.Serialize();
					}
					bool flag = party.MobileParty.Army != null && party.MobileParty.Army.LeaderParty == party.MobileParty;
					bool flag4 = !string.IsNullOrEmpty(text);
					if (flag4)
					{
						MatrixFrame identity2 = MatrixFrame.Identity;
						identity2.origin.z = identity2.origin.z + (flag ? 0.2f : 0.15f);
						identity2.rotation.RotateAboutUp(1.57079637f);
						identity2.rotation.ApplyScaleLocal(0.2f * (flag ? 1f : 0.6f));
						string text2 = "campaign_flag";
						MetaMesh bannerOfCharacter = Party_tired.GetBannerOfCharacter(new Banner(text), text2);
						bannerOfCharacter.Frame = identity2;
						gameEntity.AddMultiMesh(bannerOfCharacter, true);
					}
					strategicEntity.AddChild(gameEntity, false);
					MatrixFrame matrixFrame = MatrixFrame.Identity;
					matrixFrame.Scale(Vec3.Zero);
					bool flag5 = partyVisuals.HumanAgentVisuals != null;
					if (flag5)
					{
						partyVisuals.HumanAgentVisuals.GetEntity().SetFrame(ref matrixFrame);
					}
					bool flag6 = partyVisuals.MountAgentVisuals != null;
					if (flag6)
					{
						partyVisuals.MountAgentVisuals.GetEntity().SetFrame(ref matrixFrame);
					}
					strategicEntity.CheckResources(true);
				}
			}
			else
			{
				IEnumerable<GameEntity> children = strategicEntity.GetChildren();
				for (int i = children.Count<GameEntity>() - 1; i > -1; i--)
				{
					GameEntity gameEntity2 = children.ElementAt(i);
					bool flag7 = gameEntity2.Name != "Tent";
					if (!flag7)
					{
						strategicEntity.RemoveChild(gameEntity2, false, false, false, 0);
					}
				}
			}
		}

		private static MetaMesh GetBannerOfCharacter(Banner banner, string bannerMeshName)
		{
			MetaMesh copy = MetaMesh.GetCopy(bannerMeshName, true, false);
			for (int i = 0; i < copy.MeshCount; i++)
			{
				Mesh meshAtIndex = copy.GetMeshAtIndex(i);
				bool flag = !meshAtIndex.HasTag("dont_use_tableau");
				if (flag)
				{
					Material material = meshAtIndex.GetMaterial();
					Material tableauMaterial = null;
					Tuple<Material, BannerCode> key = new Tuple<Material, BannerCode>(material, BannerCode.CreateFrom(banner));
					bool flag2 = MapScreen.Instance._characterBannerMaterialCache.ContainsKey(key);
					if (flag2)
					{
						tableauMaterial = MapScreen.Instance._characterBannerMaterialCache[key];
					}
					else
					{
						tableauMaterial = material.CreateCopy();
						Action<Texture> action = delegate (Texture tex)
						{
							tableauMaterial.SetTexture(Material.MBTextureType.DiffuseMap2, tex);
							uint num = (uint)tableauMaterial.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
							ulong shaderFlags = tableauMaterial.GetShaderFlags();
							tableauMaterial.SetShaderFlags(shaderFlags | (ulong)num);
						};
						banner.GetTableauTextureLarge(action);
						MapScreen.Instance._characterBannerMaterialCache[key] = tableauMaterial;
					}
					meshAtIndex.SetMaterial(tableauMaterial);
				}
			}
			return copy;
		}
	}
}
