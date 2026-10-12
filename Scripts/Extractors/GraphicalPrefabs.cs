using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using DataExtractor.Utilities.Extensions;
using UnityEngine;

namespace DataExtractor.Extractors {
	public class GraphicalPrefabs : Extractor {
		public override string Name => "graphicalPrefabs";

		public override JToken Extract() {
			var prefabDataBlocks = ScriptableData.GetDataBlocks<GraphicalObjectDataBlock>()
				.Where(dataBlock => dataBlock.prefab != null && dataBlock.prefab.GetComponent<EntityMonoBehaviour>() != null)
				.GroupBy(dataBlock => dataBlock.address)
				.Select(group => group.First())
				.OrderBy(dataBlock => dataBlock.prefab.GetComponent<EntityMonoBehaviour>().name);

			var data = prefabDataBlocks.Select(dataBlock => {
				var prefab = dataBlock.prefab.GetComponent<EntityMonoBehaviour>();
				var entityMonoBehaviour = prefab.GetComponent<EntityMonoBehaviour>();
				var spriteGradientMapFromPaintColor = prefab.GetComponent<SpriteGradientMapFromPaintColor>();
				
				return new {
					Guid = dataBlock.address.ToString(),
					InternalName = prefab.name,
					EntityMonoBehaviour = new {
						SoundOptions = entityMonoBehaviour.soundOptions,
						Lights = prefab.GetComponentsInChildren<Light>(true).Select(light => {
							var flickerEffect = light.gameObject.GetComponent<LightFlickerEffect>();

							var colors = new List<Color>();
							if (spriteGradientMapFromPaintColor != null) {
								var matchingEntry = spriteGradientMapFromPaintColor.lights.FirstOrDefault(x => x.lightSource == light);
								if (matchingEntry.colors != null)
									colors.AddRange(matchingEntry.colors);
							}
							if (colors.Count == 0)
								colors.Add(light.color);
							
							return new {
								Colors = colors.Select(x => x.ToHex()),
								Range = light.range,
								Intensity = new {
									Min = flickerEffect != null ? flickerEffect.minIntensity : light.intensity,
									Max = flickerEffect != null ? flickerEffect.maxIntensity : light.intensity
								}
							};
						})
					}
				};
			});

			return Serialize(data);
		}
	}
}