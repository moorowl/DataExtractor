using Newtonsoft.Json.Linq;
using System.Linq;

namespace DataExtractor.Extractors {
	public class BehaviourTags : Extractor {
		public override string Name => "behaviourTags";

		public override JToken Extract() {
			var behaviourTags = ScriptableData.GetDataBlocks<BehaviourTagsDataBlock>();

			var data = behaviourTags.OrderBy(behaviourTag => behaviourTag.name)
				.Select(behaviourTag => new {
					Guid = behaviourTag.address.guid,
					InternalName = behaviourTag.name,
					WantsToAttackTags = behaviourTag.wantsToAttackTags,
					CantAttackTags = behaviourTag.cantAttackTags,
					EatsTags = behaviourTag.eatsTags
				});

			return Serialize(data);
		}
	}
}