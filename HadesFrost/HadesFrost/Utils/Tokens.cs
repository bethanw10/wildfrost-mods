using System;
using System.Collections;
using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace HadesFrost.Utils
{
    public class StatusTokenApplyX : StatusEffectApplyX, StatusIcons.IStatusToken
    {
        [Flags]
        public enum PlayFromFlags
        {
            None = 0,
            Board = 1,
            Hand = 2,
            Draw = 4,
            Discard = 8
        }

        public static readonly string Key_Snowed = "websiteofsites.wildfrost.pokefrost.buttonSnowed";
        public static readonly string Key_Inked = "websiteofsites.wildfrost.pokefrost.buttonInked";
        public static readonly string Key_Generic = "websiteofsites.wildfrost.pokefrost.buttonGeneric";
        public static readonly string Key_Autotomize = "websiteofsites.wildfrost.pokefrost.buttonAutotomize";

        public string genericPopup;

        //[PokeLocalizer]
        public static void DefineStrings()
        {
            StringTable tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
            tooltips.SetString(Key_Snowed, "Snowed!");
            tooltips.SetString(Key_Inked, "Inked!");
            tooltips.SetString(Key_Generic, "Not yet!");
            tooltips.SetString(Key_Autotomize, "Please recycle!");
        }

        public PlayFromFlags playFrom = PlayFromFlags.Board;
        public bool finiteUses = false;
        public bool oncePerTurn = false;
        protected bool unusedThisTurn = true;
        public bool endTurn = false;
        public float timing = 0.2f;
        public TargetConstraint[] clickConstraints = Array.Empty<TargetConstraint>();

        public override void Init()
        {
            base.Init();
        }

        public override bool RunTurnStartEvent(Entity entity)
        {
            if (entity.data.cardType.name == "Leader")
            {
                unusedThisTurn = true;
            }
            return base.RunTurnStartEvent(entity);
        }

        public virtual void RunButtonClicked()
        {
            Common.Log("here!!");
            if (References.Battle == null)
            {
                return;
            }

            Common.Log("here!! 2");
            if (target.IsSnowed)
            {
                PopupText(Key_Snowed);
                return;
            }

            Common.Log("here!! 3");
            if (target.silenced)
            {
                PopupText(Key_Inked);
                return;
            }

            Common.Log("here!! 4");
            foreach (var constraint in clickConstraints)
            {
                if (!constraint.Check(target))
                {
                    PopupText(genericPopup ?? Key_Generic);
                    return;
                }
            }

            Common.Log("here!!5");
            if (References.Battle.phase == Battle.Phase.Play
                && CorrectPlace()
                && !target.IsSnowed
                && target.owner == References.Player
                && !target.silenced
                && (!oncePerTurn || unusedThisTurn))
            {
                target.StartCoroutine(ButtonClicked());
                unusedThisTurn = false;
            }
        }

        public virtual void PopupText(string s)
        {
            NoTargetTextSystem noText = GameSystem.FindObjectOfType<NoTargetTextSystem>();
            if (noText != null)
            {
                TMP_Text textElement = noText.textElement;
                StringTable tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
                textElement.text = tooltips.GetString(s).GetLocalizedString();
                noText.PopText(target.transform.position);
            }
        }

        public bool CheckFlag(PlayFromFlags flag) => (playFrom & flag) != 0;

        public virtual bool CorrectPlace()
        {
            if (CheckFlag(PlayFromFlags.Board) && Battle.IsOnBoard(target))
            {
                return true;
            }
            if (CheckFlag(PlayFromFlags.Hand) && References.Player.handContainer.Contains(target))
            {
                return true;
            }
            if (CheckFlag(PlayFromFlags.Draw) && target.preContainers.Contains(References.Player.drawContainer))
            {
                return true;
            }
            if (CheckFlag(PlayFromFlags.Discard) && target.preContainers.Contains(References.Player.discardContainer))
            {
                return true;
            }
            return false;
        }

        //Main Code
        public int fixedAmount = 0;
        public int hitDamage = 0;

        public IEnumerator ButtonClicked()
        {
            Common.Log("here!! alt");
            if (hitDamage != 0)
            {
                List<Entity> enemies = GetTargets();
                int trueAmount = (hitDamage == -1) ? count : hitDamage;
                foreach (Entity enemy in enemies)
                {
                    if (enemy.IsAliveAndExists())
                    {
                        Hit hit = new Hit(target, enemy, trueAmount);
                        hit.canRetaliate = false;
                        yield return hit.Process();
                    }

                }

            }
            yield return Run(GetTargets(), fixedAmount);
            List<StatusTokenApplyXListener> listeners = FindListeners();
            foreach (StatusTokenApplyXListener listener in listeners)
            {
                yield return listener.Run();
            }
            target.display.promptUpdateDescription = true;
            yield return PostClick();
        }

        public List<StatusTokenApplyXListener> FindListeners()
        {
            List<StatusTokenApplyXListener> listeners = new List<StatusTokenApplyXListener>();
            foreach (StatusEffectData status in target.statusEffects)
            {
                if (status is StatusTokenApplyXListener status2)
                {
                    if (status2.type == type + "_listener")
                    {
                        listeners.Add(status2);
                    }
                }
            }
            return listeners;
        }

        public virtual IEnumerator PostClick()
        {
            if (finiteUses)
            {
                count--;
                if (count == 0)
                {
                    yield return Remove();
                }
                target.promptUpdate = true;
            }
            if (endTurn)
            {
                yield return Sequences.Wait(timing);
                References.Player.endTurn = true;
            }
        }

        public void ButtonCreate(StatusIcons.HadesStatusIcon icon)
        {
            return;
        }
    }

    public class StatusTokenApplyXListener : StatusEffectApplyX
    {
        public IEnumerator Run()
        {
            yield return Run(GetTargets());
        }
    }
}