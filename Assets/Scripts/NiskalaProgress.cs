public static class NiskalaProgress
{
    // =========================================
    // INVENTORY
    // =========================================

    public static bool tepakSirihCollected;
    public static bool bakulCollected;
    public static bool catatanCikSimaCollected;
    // Status investigasi artefak
    public static bool tepakSirihInvestigated;
    public static bool bakulInvestigated;
    public static bool catatanCikSimaInvestigated;


    // =========================================
    // RUANG PENYIMPANAN
    // =========================================

    public static int gentongClickCount;
    public static bool bakulUnlocked;
    public static bool gentongShifted;


    // =========================================
    // KAMAR TIDUR - LEMARI
    // =========================================

    public static int[] lockerCode = { 1, 1, 1, 1 };

    public static bool lockerDialogueShown;
    public static bool lockerUnlocked;


    // =========================================
    // KAMAR TIDUR - INTERAKSI KHUSUS
    // =========================================

    public static bool mirrorClickedOnce;
    public static bool mirrorIsBroken;
    public static bool scarfAlreadyClicked;


    // =========================================
    // RUANG TAMU
    // =========================================

    public static bool ruangTamuOpeningDialogueShown;
    public static bool lukisanClickedOnce;
    public static bool perhiasanClickedOnce;
    public static bool sisirClickedOnce;
    public static bool senjataClickedOnce;

    // =========================================
    // RUANG MAKAN
    // =========================================

    public static bool alatMakanClickedOnce;
    public static bool gelasClickedOnce;
    public static bool tempayanClickedOnce;

    // =========================================
    // RUANG TENGAH
    // =========================================

    public static bool patungClickedOnce;
    public static bool kainClickedOnce;
    public static bool tepakSirihPuzzleCompleted;
    public static bool tepakSirihIntroShown;

    // =========================================
    // RESET SELURUH PROGRES
    // =========================================

    public static void ResetAll()
    {
        // Inventory
        tepakSirihCollected = false;
        bakulCollected = false;
        catatanCikSimaCollected = false;

        // Status investigasi artefak
        tepakSirihInvestigated = false;
        bakulInvestigated = false;
        catatanCikSimaInvestigated = false;

        // Ruang Penyimpanan
        gentongClickCount = 0;
        bakulUnlocked = false;
        gentongShifted = false;

        // Kamar Tidur - Lemari
        lockerCode = new int[] { 1, 1, 1, 1 };
        lockerDialogueShown = false;
        lockerUnlocked = false;

        // Kamar Tidur - Interaksi Khusus
        mirrorClickedOnce = false;
        mirrorIsBroken = false;
        scarfAlreadyClicked = false;

        // Ruang Tamu
        ruangTamuOpeningDialogueShown = false;
        lukisanClickedOnce = false;
        perhiasanClickedOnce = false;
        sisirClickedOnce = false;
        senjataClickedOnce = false;

        // Ruang Makan
        alatMakanClickedOnce = false;
        gelasClickedOnce = false;
        tempayanClickedOnce = false;

        // Ruang Tengah
        patungClickedOnce = false;
        kainClickedOnce = false;
        tepakSirihPuzzleCompleted = false;
        tepakSirihIntroShown = false;
    }
}