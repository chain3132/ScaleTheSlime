#if UNITY_EDITOR
using UnityEditor;

namespace ProjectEditor
{
    
    [InitializeOnLoad]
    internal static class SortifyTeamSync
    {
        private const string SaveInProjectKey = "Sortify_SaveInProject";

        static SortifyTeamSync()
        {
            if (!EditorPrefs.GetBool(SaveInProjectKey, false))
                EditorPrefs.SetBool(SaveInProjectKey, true);
        }
    }
}
#endif

