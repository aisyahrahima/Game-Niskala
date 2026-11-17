public static class LoreOrderFlow
{
    // Ruangan tempat artefak ketiga diinvestigasi.
    public static string returnRoomAfterFullLore;

    // Penanda bahwa FullLore harus kembali ke ruangan tersebut.
    public static bool pendingFullLoreReturn;

    // Menandakan bahwa pemain sudah menyelesaikan
    // dialog investigasi artefak ketiga dan harus
    // menutup zoom sebelum masuk ke LoreOrder.
    public static bool pendingLoreOrderIntro;

    public static void Clear()
    {
        returnRoomAfterFullLore = "";
        pendingFullLoreReturn = false;
        pendingLoreOrderIntro = false;
    }
}