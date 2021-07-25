using System.Threading.Tasks;
using UnityEngine;

public static class SystemUtility {
    public static async Task<GameConfig> GetGameConfig() {
        ResourceRequest request = Resources.LoadAsync<GameConfig>("GameConfig/GameConfig");
        while (!request.isDone) {
            await Task.Delay(1);
        }

        GameConfig gc = request.asset as GameConfig;
        if (gc == null) {
            return null;
        }

        return gc;
    }
}
