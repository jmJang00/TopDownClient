using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetEntity
{
    Character _character;
    Transform _headAnchor;
    UI_NamePlate _namePlate;
    public UI_NamePlate NamePlate {  get { return _namePlate; } }

    public int AccountId { get; set; }
    public bool IsBot { get; set; }

    public override void Init()
    {
        base.Init();
        _character = GetComponent<Character>();
        _headAnchor = transform.Find("HeadAnchor");
    }

    public override void OnSpawn(int tick)
    {
        if (AccountId == AccountManager.Instance.AccountId)
        {
            //MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, _character);
            //TopDownEngineEvent.Trigger(TopDownEngineEventTypes.SpawnCharacterStarts, null);

            _character.RespawnAt(transform.position, Character.FacingDirections.East);
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.SpawnComplete, _character);

            MMSetFeedbackRangeCenterEvent.Trigger(_character.transform);

            Spectate();
        }

        _character.RespawnAt(transform.position, Character.FacingDirections.East);
        if (!IsBot)
        {
            string nickname = AccountManager.Instance.GetNickname(AccountId);
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = "Test";
            }
            _namePlate = WorldUIManager.Instance.CreateNamePlate(_headAnchor, nickname);
        }
        else
        {
            _namePlate = WorldUIManager.Instance.CreateNamePlate(_headAnchor, "Bot");
        }

        PlayerManager.Instance.AddPlayer(this);

        base.OnSpawn(tick);
    }

    public void Spectate()
    {
        MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, _character);
        MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
        MMGameEvent.Trigger("CameraBound");
    }

    public override void OnDespawn()
    {
        base.OnDespawn();

        if (AccountId == AccountManager.Instance.AccountId)
        {
            GameScene scene = NetworkManager.Instance.game;
            scene.gameSelectUI.ShowDefeat();
        }

        PlayerManager.Instance.RemovePlayer(entityId);

        WorldUIManager.Instance.RemoveNamePlate(_namePlate);
    }
}
