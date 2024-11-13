using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;

namespace HadesFrost.ButtonStatuses
{
    public class StatusHexApplyX : StatusEffectApplyX, IStatusHex
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

        public string GenericPopup;

        public int FixedAmount = 0;
        public int HitDamage = 0;

        //[PokeLocalizer]
        public static void DefineStrings()
        {
            var tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
            tooltips.SetString(Key_Snowed, "Snowed!");
            tooltips.SetString(Key_Inked, "Inked!");
            tooltips.SetString(Key_Generic, "Not yet!");
            tooltips.SetString(Key_Autotomize, "Please recycle!");
        }

        public bool OncePerTurn = false;
        public int MagickCost = 0;

        private readonly PlayFromFlags playFrom = PlayFromFlags.Board;
        private readonly bool finiteUses = false;
        private bool unusedThisTurn = true;
        private readonly bool endTurn = false;
        private readonly float timing = 0.2f;
        private readonly TargetConstraint[] clickConstraints = Array.Empty<TargetConstraint>();

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
            if (References.Battle == null)
            {
                return;
            }

            var magick = target.statusEffects.SingleOrDefault(s => s.name == "bethanw10.hadesfrost.Magick");
            var magickCount = magick?.count ?? 0;

            if (magickCount < MagickCost)
            {
                PopupText("Not enough<sprite name=magickicon>!");
                return;
            }

            var targets = GetTargets();
            if (targets.Count == 0)
            {
                PopupText("No targets available!");
                return;
            }

            foreach (var constraint in clickConstraints)
            {
                if (!constraint.Check(target))
                {
                    PopupText(GenericPopup ?? Key_Generic);
                    return;
                }
            }

            if (References.Battle.phase == Battle.Phase.Play
                && CorrectPlace()
                && target.owner == References.Player
                && (!OncePerTurn || unusedThisTurn))
            {
                target.StartCoroutine(ButtonClicked());
                unusedThisTurn = false;

                // if (magick != null)
                // {
                //     magick.count -= MagickCost;
                //     this.target.PromptUpdate();
                // }

                target.StartCoroutine(magick?.RemoveStacks(this.MagickCost, false));
                //magick?.RemoveStacks(this.MagickCost, false);
            }
        }

        protected virtual void PopupText(string s)
        {
            var noText = FindObjectOfType<NoTargetTextSystem>();
            if (noText != null)
            {
                var textElement = noText.textElement;
                textElement.text = s;
                noText.PopText(target.transform.position);
            }
        }

        // protected virtual void PopupText(string s)
        // {
        //     var noText = FindObjectOfType<NoTargetTextSystem>();
        //     if (noText != null)
        //     {
        //         var textElement = noText.textElement;
        //         var tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
        //         textElement.text = tooltips.GetString(s).GetLocalizedString();
        //         noText.PopText(target.transform.position);
        //     }
        // }

        private bool CheckFlag(PlayFromFlags flag) => (playFrom & flag) != 0;

        protected virtual bool CorrectPlace()
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

        public IEnumerator ButtonClicked()
        {
            if (HitDamage != 0)
            {
                var enemies = GetTargets();
                var trueAmount = (HitDamage == -1) ? count : HitDamage;
                foreach (var enemy in enemies)
                {
                    if (enemy.IsAliveAndExists())
                    {
                        var hit = new Hit(target, enemy, trueAmount)
                        {
                            damageType = "hex",
                            canRetaliate = false
                        };
                        yield return hit.Process();
                    }

                }

            }
            yield return Run(GetTargets(), FixedAmount);
            var listeners = FindListeners();
            foreach (var listener in listeners)
            {
                yield return listener.Run();
            }
            target.display.promptUpdateDescription = true;
            yield return PostClick();
        }

        public List<StatusHexApplyXListener> FindListeners()
        {
            var listeners = new List<StatusHexApplyXListener>();
            foreach (var status in target.statusEffects)
            {
                if (status is StatusHexApplyXListener status2)
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

        public void ButtonCreate(HexStatusIcon icon)
        {
        }
    }
}