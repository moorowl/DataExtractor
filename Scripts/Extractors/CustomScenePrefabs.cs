using System.Collections.Generic;
using DataExtractor.DataStructures;
using DataExtractor.Utilities;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;

namespace DataExtractor.Extractors {
	public class CustomScenePrefabs : Extractor {
		public override string Name => "customScenePrefabs";

		public override JToken Extract() {
			var objects = ScriptableData.GetDataBlocks<EntityAuthoringDataBlock>()
				.Select(dataBlock => new ObjectDataFromPrefab(dataBlock))
				.OrderBy(data => data.SortKey)
				.ToList();

			var baseObjects = objects
				.Where(data => !data.HasComponent<CustomScenePrefabAuthoring>())
				.Select(data => {
					var objectInfo = data.ObjectInfo;
					var objectData = new ObjectDataCD {
						objectID = objectInfo.objectID,
						variation = objectInfo.variation,
						amount = 1
					};

					var serializedComponents = data.Components.OrderBy(entry => entry.Key.Name)
						.ToDictionary(entry => entry.Key.Name, entry => Serialize(entry.Value, DefaultSettingsForObjects) as JObject);

					return (objectData, serializedComponents);
				})
				.ToDictionary(entry => entry.objectData, entry => entry.serializedComponents);

			var customScenePrefabs = ScriptableData.GetDataBlocks<CustomSceneDataBlock>().SelectMany(scene => scene.authObjects)
				.Select(entityRef => new ObjectDataFromPrefab(entityRef.Get()))
				.Where(data => data.HasComponent<CustomScenePrefabAuthoring>())
				.GroupBy(data => Utils.GetUniqueScenePrefabName(data.Authoring))
				.Select(group => group.First())
				.OrderBy(data => data.SortKey)
				.Select(data => {
					var baseSerializedComponents = baseObjects.GetValueOrDefault(new ObjectDataCD {
						objectID = data.ObjectInfo.objectID,
						variation = data.ObjectInfo.variation
					}) ?? baseObjects.GetValueOrDefault(new ObjectDataCD {
						objectID = data.ObjectInfo.objectID
					});
					var serializedComponents = data.Components.OrderBy(entry => entry.Key.Name)
						.ToDictionary(entry => entry.Key.Name, entry => Serialize(entry.Value, DefaultSettingsForObjects) as JObject);

					var overrideComponents = serializedComponents
						.Where(x => !baseSerializedComponents.ContainsKey(x.Key) || x.Value.ToString() != baseSerializedComponents[x.Key].ToString())
						.ToDictionary(x => x.Key, x => x.Value);

					return new {
						AuthoringPrefabName = Utils.GetUniqueScenePrefabName(data.Authoring),
						Object = new {
							Id = data.ObjectInfo.objectID,
							Variation = data.ObjectInfo.variation
						},
						ComponentOverrides = overrideComponents
					};
				});

			return Serialize(customScenePrefabs);
		}
	}
}