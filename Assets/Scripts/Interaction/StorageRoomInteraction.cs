using System.Collections;
using UnityEngine;

public class StorageRoomInteraction : MonoBehaviour
{
    [Header("Catatan")]
    public GameObject catatan;
    public GameObject catatanZoom;
    public GameObject arrowBack;

    [Header("Dialog")]
    public GameObject dialogueNextButton;

    [Header("Gentong")]
    public GameObject gentong;
    public GameObject jumpscare;
    public GameObject bakul;

    // =========================================================
    // STATUS EVENT
    // =========================================================

    private bool eventRunning = false;

    // Menentukan event apa yang sedang berjalan
    private string currentEvent = "";

    // =========================================================
    // CATATAN
    // =========================================================

    private int catatanDialogueIndex = 0;

    private string[] catatanDialogue =
    {
        "hmm...",
        "Sepertinya ada maksud dari dokumen ini..."
    };


    // =========================================================
    // GENTONG
    // =========================================================

    private int gentongClickCount = 0;


    // =========================================================
    // CATATAN - MULAI
    // =========================================================

    public void OpenCatatan()
    {
        // Jangan bisa membuka Catatan jika ada event lain
        if (eventRunning)
            return;

        eventRunning = true;
        currentEvent = "Catatan";

        // Sembunyikan Catatan biasa
        catatan.SetActive(false);

        // Tampilkan Catatan Zoom
        catatanZoom.SetActive(true);

        // ArrowBack tidak boleh digunakan
        arrowBack.SetActive(false);

        // DialogueNextButton disembunyikan dulu
        dialogueNextButton.SetActive(false);

        // Mulai dari dialog pertama
        catatanDialogueIndex = 0;

        // Tunggu 2,5 detik sebelum dialog
        StartCoroutine(StartCatatanDialogue());
    }


    private IEnumerator StartCatatanDialogue()
    {
        yield return new WaitForSeconds(2.5f);

        // Tampilkan tombol dialog
        dialogueNextButton.SetActive(true);

        ShowCatatanDialogue();
    }


    // =========================================================
    // CATATAN - NEXT DIALOG
    // =========================================================

    public void NextCatatanDialogue()
    {
        // Pastikan yang sedang berjalan memang event Catatan
        if (currentEvent != "Catatan")
            return;

        catatanDialogueIndex++;

        // Masih ada dialog berikutnya
        if (catatanDialogueIndex < catatanDialogue.Length)
        {
            ShowCatatanDialogue();
        }
        else
        {
            // Dialog terakhir selesai
            FinishCatatanEvent();
        }
    }


    private void ShowCatatanDialogue()
    {
        Debug.Log("ARKA: " + catatanDialogue[catatanDialogueIndex]);
    }


    // =========================================================
    // CATATAN - SELESAI
    // =========================================================

    private void FinishCatatanEvent()
    {
        Debug.Log("Event Catatan selesai");

        dialogueNextButton.SetActive(false);

        eventRunning = false;
        currentEvent = "";

        // Setelah dialog selesai,
        // pemain boleh menggunakan ArrowBack
        arrowBack.SetActive(true);
    }


    // =========================================================
    // CATATAN - KELUAR ZOOM
    // =========================================================

    public void CloseCatatanZoom()
    {
        // Jangan bisa keluar selama event/dialog berlangsung
        if (eventRunning)
            return;

        catatanZoom.SetActive(false);
        catatan.SetActive(true);

        arrowBack.SetActive(true);
    }


    // =========================================================
    // GENTONG - KLIK
    // =========================================================

    public void ClickGentong()
    {
        // Jangan menerima klik jika ada event yang sedang berjalan
        if (eventRunning)
            return;

        eventRunning = true;

        // ArrowBack langsung hilang
        arrowBack.SetActive(false);

        // DialogueNextButton belum boleh muncul
        dialogueNextButton.SetActive(false);

        // Tambah jumlah klik Gentong
        gentongClickCount++;

        // Tentukan event yang sedang berjalan
        currentEvent = "Gentong" + gentongClickCount;

        Debug.Log("Gentong diklik: " + gentongClickCount);

        StartCoroutine(GentongEvent());
    }


    // =========================================================
    // GENTONG - EVENT
    // =========================================================

    private IEnumerator GentongEvent()
    {
        // =====================================================
        // GENTONG 1
        // =====================================================

        if (gentongClickCount == 1)
        {
            Debug.Log("Event Gentong 1 dimulai.");

            // Tunggu 2,5 detik
            yield return new WaitForSeconds(2.5f);

            // Tampilkan tombol dialog
            dialogueNextButton.SetActive(true);

            // Hanya SATU dialog
            Debug.Log("ARKA: Apa itu?");
        }


        // =====================================================
        // GENTONG 2
        // =====================================================

        else if (gentongClickCount == 2)
        {
            Debug.Log("Event Gentong 2 dimulai.");

            // Tunggu 2,5 detik
            yield return new WaitForSeconds(2.5f);

            // Event bisikan
            Debug.Log("SFX: Bisikan 'Mayang...'");

            // Tunggu 2,5 detik setelah event
            yield return new WaitForSeconds(2.5f);

            dialogueNextButton.SetActive(true);

            // Hanya SATU dialog
            Debug.Log("ARKA: Siapa...?");
        }


        // =====================================================
        // GENTONG 3
        // =====================================================

        else if (gentongClickCount == 3)
        {
            Debug.Log("Event Gentong 3 dimulai.");

            // Tunggu 2,5 detik
            yield return new WaitForSeconds(2.5f);

            // Jumpscare muncul
            jumpscare.SetActive(true);

            Debug.Log("JUMPSCARE MUNCUL");

            // Jumpscare berlangsung 2,5 detik
            yield return new WaitForSeconds(2.5f);

            // Jumpscare hilang otomatis
            jumpscare.SetActive(false);

            Debug.Log("JUMPSCARE SELESAI");

            // Tunggu 2,5 detik
            yield return new WaitForSeconds(2.5f);

            dialogueNextButton.SetActive(true);

            // Hanya SATU dialog
            Debug.Log("ARKA: Tadi... apa itu?");
        }


        // =====================================================
        // GENTONG 4
        // =====================================================

        else if (gentongClickCount == 4)
        {
            Debug.Log("Event Gentong 4 dimulai.");

            // Tunggu 2,5 detik
            yield return new WaitForSeconds(2.5f);

            // Gentong menghilang / bergeser
            gentong.SetActive(false);

            // Bakul mulai terlihat
            bakul.SetActive(true);

            Debug.Log("Bakul sekarang terlihat.");

            // Tunggu 2,5 detik
            yield return new WaitForSeconds(2.5f);

            dialogueNextButton.SetActive(true);

            // Hanya SATU dialog
            Debug.Log("ARKA: Bakul...?");
        }
    }


    // =========================================================
    // DIALOGUE NEXT BUTTON
    // =========================================================

    public void NextDialogue()
    {
        // Tidak ada event
        if (!eventRunning)
            return;

        // =====================================================
        // CATATAN
        // =====================================================

        if (currentEvent == "Catatan")
        {
            NextCatatanDialogue();
            return;
        }


        // =====================================================
        // GENTONG
        // =====================================================

        if (currentEvent == "Gentong1")
        {
            FinishGentongEvent(1);
            return;
        }

        if (currentEvent == "Gentong2")
        {
            FinishGentongEvent(2);
            return;
        }

        if (currentEvent == "Gentong3")
        {
            FinishGentongEvent(3);
            return;
        }

        if (currentEvent == "Gentong4")
        {
            FinishGentongEvent(4);
            return;
        }
    }


    // =========================================================
    // GENTONG - SELESAI
    // =========================================================

    private void FinishGentongEvent(int eventNumber)
    {
        Debug.Log("Event Gentong " + eventNumber + " selesai.");

        // Sembunyikan tombol dialog
        dialogueNextButton.SetActive(false);

        // Event selesai
        eventRunning = false;
        currentEvent = "";

        // ArrowBack muncul kembali
        arrowBack.SetActive(true);
    }
}