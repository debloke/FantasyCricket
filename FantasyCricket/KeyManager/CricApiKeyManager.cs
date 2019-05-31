using System.Collections.Concurrent;

namespace FantasyCricket.KeyManager
{
    public class CricApiKeyManager
    {
        ConcurrentDictionary<string, int> apiKeys = new ConcurrentDictionary<string, int>();

    }
}
