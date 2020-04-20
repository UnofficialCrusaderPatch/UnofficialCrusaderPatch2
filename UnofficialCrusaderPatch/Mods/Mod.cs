namespace UCP.Patching
{
    public class Mod
    {
        /**
         * The HD Change of the modification.
         */
        protected Change change;
        
        /**
         * The Extreme Change of the modification.
         */
        protected Change extremeChange;
        
        /**
         * The HD Change of the modification.
         */
        public Change Change { get { return change; } }
        
        /**
         * The Extreme Change of the modification.
         */
        public Change ExtremeChange { get { return extremeChange; } }

        public Mod()
        {
            change = CreateChange();
            //extremeChange = CreateExtremeChange();
        }

        /**
         * Initialize Extreme change on demand.
         * This is needed due to how the UI is stitched together tightly with Changes,
         * and how the GlobalLabels work.
         * Extreme changes are not initialized, only the HD changes at startup, and the
         * UI is built with the HD changes and NOT with the Extreme changes.
         *
         * Extreme changes are inited when the patcher runs on the Extreme file, and
         * before that, the GlobalLabels collection is emptied to avoid duplication
         * exception.
         */
        public void InitExtremeChange()
        {
            extremeChange = CreateExtremeChange();
        }

        /**
         * Creates the HD change, but also should cache it.
         */
        protected virtual Change CreateChange()
        {
            // Overridden.
            return null;
        }

        /**
         * Creates the Extreme change, but also should cache it.
         */
        protected virtual Change CreateExtremeChange()
        {
            // Overridden.
            return null;
        }
    }
}