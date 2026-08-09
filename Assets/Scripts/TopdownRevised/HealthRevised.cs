using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class HealthRevised : Health
{
    public override void Kill()
    {
        if (ImmuneToDamage)
        {
            return;
        }

        if (_character != null)
        {
            // we set its dead state to true
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Dead);
            _character.Reset();

            if (_character.CharacterType == Character.CharacterTypes.Player)
            {
                //아래만 삭제하려고 옮겨옴.
                //TopDownEngineEvent.Trigger(TopDownEngineEventTypes.PlayerDeath, _character);
            }
        }
        SetHealth(0);

        // we prevent further damage
        StopAllDamageOverTime();
        DamageDisabled();

        DeathMMFeedbacks?.PlayFeedbacks(this.transform.position);

        // Adds points if needed.
        if (PointsWhenDestroyed != 0)
        {
            // we send a new points event for the GameManager to catch (and other classes that may listen to it too)
            TopDownEnginePointEvent.Trigger(PointsMethods.Add, PointsWhenDestroyed);
        }

        if (_hasDeathParameter)
        {
            TargetAnimator.SetTrigger(_deathAnimatorParameter);
        }
        // we make it ignore the collisions from now on
        if (DisableCollisionsOnDeath)
        {
            if (_collider2D != null)
            {
                _collider2D.enabled = false;
            }
            if (_collider3D != null)
            {
                _collider3D.enabled = false;
            }

            // if we have a controller, removes collisions, restores parameters for a potential respawn, and applies a death force
            if (_controller != null)
            {
                _controller.CollisionsOff();
            }

            if (DisableChildCollisionsOnDeath)
            {
                foreach (Collider2D collider in this.gameObject.GetComponentsInChildren<Collider2D>())
                {
                    collider.enabled = false;
                }
                foreach (Collider collider in this.gameObject.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = false;
                }
            }
        }

        if (ChangeLayerOnDeath)
        {
            gameObject.layer = LayerOnDeath.LayerIndex;
            if (ChangeLayersRecursivelyOnDeath)
            {
                this.transform.ChangeLayersRecursively(LayerOnDeath.LayerIndex);
            }
        }

        OnDeath?.Invoke();
        MMLifeCycleEvent.Trigger(this, MMLifeCycleEventTypes.Death);

        if (DisableControllerOnDeath && (_controller != null))
        {
            _controller.enabled = false;
        }

        if (DisableControllerOnDeath && (_characterController != null))
        {
            _characterController.enabled = false;
        }

        if (DisableModelOnDeath && (Model != null))
        {
            Model.SetActive(false);
        }

        if (DelayBeforeDestruction > 0f)
        {
            Invoke("DestroyObject", DelayBeforeDestruction);
        }
        else
        {
            // finally we destroy the object
            DestroyObject();
        }
    }
}


