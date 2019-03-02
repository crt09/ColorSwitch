﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace ColorSwitch.Windows.GameCore.Entities.Special {
	public class Star : TouchableEntity {

		private Texture2D starTexture;
		private Sprite starSprite;

		public override Vector2 realSize => starTexture.Bounds.Size.ToVector2();

		public Star() : base("star") { }

		public override void onAddedToScene() {
			starTexture = scene.content.Load<Texture2D>("ColorEntities/star");
			starSprite = new Sprite(starTexture);
			addComponent(starSprite);

			var collider = new CircleCollider(starTexture.Height / 2);
			addComponent(collider);
		}

		public override void SendState(Entity sender) {
			if (sender is Player player) {
				var starCollider = getComponent<Collider>();
				var playerCollider = player.getComponent<Collider>();
				if (starCollider.collidesWith(playerCollider, out CollisionResult result)) {					
					player.score++;
					destroy();
				}
			}
		}
	}
}