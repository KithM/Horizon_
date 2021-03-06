﻿using UnityEngine;

public class SpawnManager : MonoBehaviour {

	// Instance
	public static SpawnManager current;

	void Awake(){
		current = this;
	}

	public void SpawnShip(Ship.Type shipType, Ship.Faction shipFaction, Vector2 position, bool isPlayer = false){

		Sprite shipSprite;
		Sprite bulletSprite;
		int minHealth;
		int maxHealth;
		float engineOffset = 0f;
		float healthBarOffset = 0f;
		float shipSpeed = 0f;
		float fireRate = 0f;
		float fireDamage = 0f;
		float fireRange = 0f;
		float fireBurstCount = 1f;

		switch (shipType) {
		case Ship.Type.TraderShip:
			minHealth = 20;
			maxHealth = 50;
			engineOffset = 0f;
			healthBarOffset = 1.25f;
			shipSpeed = 3.25f;
			fireRate = 3f;
			fireDamage = 5f;
			fireBurstCount = 4f;
			fireRange = 20f;

			bulletSprite = ObjectController.current.traderShipBullet;
			shipSprite = ObjectController.current.traderShip;
			break;
		case Ship.Type.PrisonShip:
			minHealth = 50;
			maxHealth = 150;
			engineOffset = 0.25f;
			healthBarOffset = 1.625f;
			shipSpeed = 2.5f;
			fireRate = 3f;
			fireDamage = 7.5f;
			fireBurstCount = 5f;
			fireRange = 22f;

			bulletSprite = ObjectController.current.prisonShipBullet;
			shipSprite = ObjectController.current.prisonShip;
			break;
		case Ship.Type.FighterShip:
			minHealth = 50;
			maxHealth = 100;
			engineOffset = -0.125f;
			healthBarOffset = 1f;
			shipSpeed = 3.5f;
			fireRate = 2f;
			fireDamage = 5f;
			fireBurstCount = 9f;
			fireRange = 25f;

			bulletSprite = ObjectController.current.fighterShipBullet;
			shipSprite = ObjectController.current.fighterShip;
			break;
		case Ship.Type.AdvancedFighterShip:
			minHealth = 100;
			maxHealth = 350;
			engineOffset = 0.175f;
			healthBarOffset = 1.275f;
			shipSpeed = 4.75f;
			fireRate = 2.5f;
			fireDamage = 15f;
			fireBurstCount = 4f;
			fireRange = 30f;

			bulletSprite = ObjectController.current.advancedFighterShipBullet;
			shipSprite = ObjectController.current.advancedFighterShip;
			break;
		case Ship.Type.HeavyFighterShip:
			minHealth = 250;
			maxHealth = 500;
			engineOffset = 0.300f;
			healthBarOffset = 1.75f;
			shipSpeed = 2.325f;
			fireRate = 4.5f;
			fireDamage = 25f;
			fireBurstCount = 3f;
			fireRange = 36f;

			bulletSprite = ObjectController.current.heavyFighterShipBullet;
			shipSprite = ObjectController.current.heavyFighterShip;
			break;
		case Ship.Type.DestroyerShip:
			minHealth = 500;
			maxHealth = 1000;
			engineOffset = 0.500f;
			healthBarOffset = 2.00f;
			shipSpeed = 2.125f;
			fireRate = 4f;
			fireDamage = 10f;
			fireBurstCount = 8f;
			fireRange = 50f;

			bulletSprite = ObjectController.current.destroyerShipBullet;
			shipSprite = ObjectController.current.destroyerShip;
			break;
		case Ship.Type.DroneShip:
			minHealth = 25;
			maxHealth = 65;
			engineOffset = -0.5f;
			healthBarOffset = 0.75f;
			shipSpeed = 5f;
			fireRate = 2f;
			fireDamage = 2.5f;
			fireBurstCount = 12f;
			fireRange = 28f;

			bulletSprite = ObjectController.current.droneShipBullet;
			shipSprite = ObjectController.current.droneShip;
			break;
		default:
			minHealth = 50;
			maxHealth = 125;
			shipSpeed = 2.5f;
			fireDamage = 5f;
			fireRate = 2f;
			fireRange = 20f;

			bulletSprite = ObjectController.current.fighterShipBullet;
			shipSprite = ObjectController.current.fighterShip;
			break;
		}

		// Create the body and attach sprites
		var npc_body = Object.Instantiate (ObjectController.current.shipPrefab, GameController.current.canvas.transform);

		Character npc;
		if (isPlayer) {
			npc = npc_body.AddComponent <Player> ();
		} else {
			npc = npc_body.AddComponent <NPC> ();
		}

		var npc_sprite = npc_body.GetComponent<SpriteRenderer> ();
		npc_sprite.sprite = shipSprite;

		var npc_thrusters = Object.Instantiate (ObjectController.current.enginePrefab, npc_body.transform);
		npc_thrusters.transform.position = new Vector3(npc_body.transform.position.x, npc_body.transform.position.y - engineOffset, npc_body.transform.position.z);

		var npc_healthBar = Object.Instantiate (ObjectController.current.healthBarPrefab, npc_body.transform);
		npc_healthBar.transform.position = new Vector3(npc_body.transform.position.x, npc_body.transform.position.y + healthBarOffset, npc_body.transform.position.z);

		// Set up RigidBody2D and Collider
		var npc_rb = npc_body.AddComponent<Rigidbody2D> ();
		npc_rb.gravityScale = 0f;
		npc_rb.isKinematic = false;
		npc_rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

		var npc_col = npc_body.AddComponent<PolygonCollider2D> ();

		int identifier = Random.Range (100, 999);
		npc.gameObject.name = shipType + "" + identifier + " [" + shipFaction + "]";

		// Initial Setup
		npc.SetupShip (shipType, shipFaction, identifier, Random.Range (minHealth, maxHealth), shipSpeed, fireRate, fireBurstCount, fireDamage, fireRange, bulletSprite, isPlayer);
		npc_body.transform.position = position;

		// Final Setup
		npc_rb.mass = npc.MaxHealth;

		//Debug.Log ("Spawned Ship of type \'<b>" + shipType + "</b>\' and of faction \'<b>" + shipFaction + "</b>\'.");
	}

	public void SpawnRandomShip( Vector2 pos, bool isPlayer = false ){
		Ship.Type npcType = GetRandomShipType ();
		Ship.Faction npcFaction = GetRandomShipFaction();
		SpawnManager.current.SpawnShip (npcType, npcFaction, pos, isPlayer);
	}

	public void SpawnRandomAlly( Vector2 pos, bool isPlayer = false ){
		SpawnManager.current.SpawnShip (GetRandomShipType (), Ship.Faction.Ally, pos, isPlayer);
	}
	public void SpawnRandomNeutral( Vector2 pos, bool isPlayer = false ){
		SpawnManager.current.SpawnShip (GetRandomShipType (), Ship.Faction.Neutral, pos, isPlayer);
	}
	public void SpawnRandomEnemy( Vector2 pos, bool isPlayer = false ){
		SpawnManager.current.SpawnShip (GetRandomShipType (), Ship.Faction.Enemy, pos, isPlayer);
	}

	public void SpawnRandomHeavyAlly( Vector2 pos, bool isPlayer = false ){
		SpawnManager.current.SpawnShip (GetRandomHeavyShipType (), Ship.Faction.Ally, pos, isPlayer);
	}
	public void SpawnRandomHeavyNeutral( Vector2 pos, bool isPlayer = false ){
		SpawnManager.current.SpawnShip (GetRandomHeavyShipType (), Ship.Faction.Neutral, pos, isPlayer);
	}
	public void SpawnRandomHeavyEnemy( Vector2 pos, bool isPlayer = false ){
		SpawnManager.current.SpawnShip (GetRandomHeavyShipType (), Ship.Faction.Enemy, pos, isPlayer);
	}

	public void SpawnRandomLightAlly( Vector2 pos, bool isPlayer = false ){
		SpawnManager.current.SpawnShip (GetRandomLightShipType (), Ship.Faction.Ally, pos, isPlayer);
	}
	public void SpawnRandomLightNeutral( Vector2 pos, bool isPlayer = false ){
		SpawnManager.current.SpawnShip (GetRandomLightShipType (), Ship.Faction.Neutral, pos, isPlayer);
	}
	public void SpawnRandomLightEnemy( Vector2 pos, bool isPlayer = false ){
		SpawnManager.current.SpawnShip (GetRandomLightShipType (), Ship.Faction.Enemy, pos, isPlayer);
	}

	public Ship.Type GetRandomShipType(){		
		int randT = Random.Range (0, 7);
		Ship.Type npcType;

		switch (randT) {
		case 0:
			npcType = Ship.Type.FighterShip;
			break;
		case 1:
			npcType = Ship.Type.PrisonShip;
			break;
		case 2:
			npcType = Ship.Type.TraderShip;
			break;
		case 3:
			npcType = Ship.Type.AdvancedFighterShip;
			break;
		case 4:
			npcType = Ship.Type.HeavyFighterShip;
			break;
		case 5:
			npcType = Ship.Type.DestroyerShip;
			break;
		case 6:
			npcType = Ship.Type.DroneShip;
			break;
		default:
			npcType = Ship.Type.FighterShip;
			break;
		}

		return npcType;
	}

	public Ship.Faction GetRandomShipFaction(){
		int randF = Random.Range (0, 3);
		Ship.Faction npcFaction;

		switch (randF) {
		case 0:
			npcFaction = Ship.Faction.Ally;
			break;
		case 1:
			npcFaction = Ship.Faction.Neutral;
			break;
		case 2:
			npcFaction = Ship.Faction.Enemy;
			break;
		default:
			npcFaction = Ship.Faction.Neutral;
			break;
		}

		return npcFaction;
	}

	public Ship.Type GetRandomHeavyShipType(){		
		int randT = Random.Range (0, 3);
		Ship.Type npcType;

		switch (randT) {
		case 0:
			npcType = Ship.Type.AdvancedFighterShip;
			break;
		case 1:
			npcType = Ship.Type.HeavyFighterShip;
			break;
		case 2:
			npcType = Ship.Type.DestroyerShip;
			break;
		default:
			npcType = Ship.Type.AdvancedFighterShip;
			break;
		}

		return npcType;
	}

	public Ship.Type GetRandomLightShipType(){		
		int randT = Random.Range (0, 4);
		Ship.Type npcType;

		switch (randT) {
		case 0:
			npcType = Ship.Type.DroneShip;
			break;
		case 1:
			npcType = Ship.Type.FighterShip;
			break;
		case 2:
			npcType = Ship.Type.PrisonShip;
			break;
		case 3:
			npcType = Ship.Type.TraderShip;
			break;
		default:
			npcType = Ship.Type.DroneShip;
			break;
		}

		return npcType;
	}

	public Vector2 GetRandomAllyPosition(){
		return new Vector2 (Random.Range (-125f, -100f), Random.Range (-125f, 125f));
	}
	public Vector2 GetRandomEnemyPosition(){
		return new Vector2 (Random.Range (125f, 100f), Random.Range (-125f, 125f));
	}
	public Vector2 GetRandomPosition(){
		return new Vector2 (Random.Range (-125f, 125f), Random.Range (-125f, 125f));
	}
}
