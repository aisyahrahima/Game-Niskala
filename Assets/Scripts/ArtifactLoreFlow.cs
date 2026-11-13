public static class ArtifactLoreFlow
{
    public static string artifactID;
    public static string returnSceneName;

    public static bool pendingReturn;

    // Kondisi ArrowBack sebelum zoom dibuka.
    public static bool arrowBackPreviousState;
    public static bool hasArrowBackState;

    public static void Clear()
    {
        artifactID = "";
        returnSceneName = "";
        pendingReturn = false;

        arrowBackPreviousState = false;
        hasArrowBackState = false;
    }
}