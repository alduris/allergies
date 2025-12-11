namespace Allergies.Allergens
{
    internal class SlimeMoldAllergen() : SimpleEdibleItemAllergen<SlimeMold>(AbstractPhysicalObject.AbstractObjectType.SlimeMold, 0)
    {
        public override bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return base.MatchesCriteria(thing, trigger) && (!ModManager.DLCShared || (thing as SlimeMold)!.abstractPhysicalObject.type != DLCSharedEnums.AbstractObjectType.Seed);
        }
    }
}
