namespace Loxifi.Services
{
    internal static class ObjectService
    {
        public static bool IsDefault(object? obj)
        {
            if (obj is null)
            {
                return true;
            }

            Type t = obj.GetType();
            if (t.IsClass)
            {
                return false;
            }

            object d = Activator.CreateInstance(t);

            return object.Equals(d, obj);
        }
    }
}