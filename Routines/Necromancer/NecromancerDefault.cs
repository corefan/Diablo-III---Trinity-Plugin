﻿using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Necromancer
{
    public sealed class NecromancerDefault : NecroMancerBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Necromancer Generic Routine";
        public string Description => "Generic class support, casts all spells whenever possible";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => string.Empty;
        public Build BuildRequirements => null;

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;

            if (TryCursePower(out power))
                return power;

            if (TryBloodPower(out power))
                return power;

            if (TryCorpsePower(out power))
                return power;

            if (TryReanimationPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            return null;
        }

        protected override bool ShouldCommandSkeletons(out TrinityActor target)
        {
            if (base.ShouldCommandSkeletons(out target))
            {
                return Skills.Necromancer.CommandSkeletons.TimeSinceUse > 5000;
            }
            return false;
        }

        public TrinityPower GetDefensivePower()
        {
            return GetBuffPower();
        }

        public TrinityPower GetBuffPower()
        {
            // todo: what buffs does necro have?
            return null;
        }

        public TrinityPower GetDestructiblePower()
        {
            return DefaultDestructiblePower();
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (Skills.Necromancer.BloodRush.CanCast())
            {
                if (Player.CurrentHealthPct < 0.5 || Avoider.ShouldAvoid || Player.Actor.IsInCriticalAvoidance)
                    return BloodRush(Avoider.SafeSpot);
            } 

            return Walk(destination);
        }

        #region Settings      

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public NecromancerDefaultSettings Settings { get; } = new NecromancerDefaultSettings();

        public sealed class NecromancerDefaultSettings : NotifyBase, IDynamicSetting
        {
            //private SkillSettings _wrathOfTheBerserker;
            //private SkillSettings _furiousCharge;

            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(1)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get { return _emergencyHealthPct; }
                set { SetField(ref _emergencyHealthPct, value); }
            }

            //public SkillSettings WrathOfTheBerserker
            //{
            //    get { return _wrathOfTheBerserker; }
            //    set { SetField(ref _wrathOfTheBerserker, value); }
            //}

            //public SkillSettings FuriousCharge
            //{
            //    get { return _furiousCharge; }
            //    set { SetField(ref _furiousCharge, value); }
            //}

            //#region Skill Defaults

            //private static readonly SkillSettings WrathOfTheBerserkerDefaults = new SkillSettings
            //{
            //    UseMode = UseTime.Selective,
            //    Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            //};

            //private static readonly SkillSettings FuriousChargeDefaults = new SkillSettings
            //{
            //    UseMode = UseTime.Default,
            //    RecastDelayMs = 200,
            //    Reasons = UseReasons.Blocked
            //};

            //#endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                //WrathOfTheBerserker = WrathOfTheBerserkerDefaults.Clone();
                //FuriousCharge = FuriousChargeDefaults.Clone();
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}

