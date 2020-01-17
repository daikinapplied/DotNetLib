namespace RecursiveGeek.DotNetLib.Data
{
    public static class Generic
    {
        #region Functions
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            var temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static void CopyObject<T>(object sourceObject, ref T destObject)
        {
            //  If either the source, or destination is null, return
            if (sourceObject == null || destObject == null) { return; }

            //  Get the type of each object
            var sourceType = sourceObject.GetType();
            var targetType = destObject.GetType();

            //  Loop through the source properties
            foreach (var p in sourceType.GetProperties())
            {
                //  Get the matching property in the destination object
                var targetObj = targetType.GetProperty(p.Name);
                //  If there is none, skip
                if (targetObj == null) { continue; }

                //  Set the value in the destination
                targetObj.SetValue(destObject, p.GetValue(sourceObject, null), null);
            }
        }
        #endregion
    }
}
