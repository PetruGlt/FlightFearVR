using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class NPCSetupTool : EditorWindow
{
    // Adaugam un meniu in bara de sus a Unity: Tools -> Setup -> NPC Complet
    [MenuItem("Tools/Setup/Make Walker NPC")]
    public static void SetupWalkerNPC()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("⚠️ Nu ai selectat niciun obiect! Selecteaza modelele NPC din scena mai intai.");
            return;
        }

        foreach (GameObject npc in selectedObjects)
        {
            // Inregistram actiunea pentru UNDO (Ctrl+Z)
            Undo.RegisterCompleteObjectUndo(npc, "Setup NPC Walker");

            // ---------------------------------------------------------
            // 1. NAV MESH AGENT
            // ---------------------------------------------------------
            NavMeshAgent agent = GetOrAddComponent<NavMeshAgent>(npc);
            agent.speed = 1.5f;          // Viteza medie
            agent.angularSpeed = 120f;   // Rotatie
            agent.acceleration = 8f;
            agent.radius = 0.35f;        // Mai subtire sa nu se blocheze
            agent.height = 1.8f;         // Inaltime standard umana
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance; // Evitare mai fluida

            // ---------------------------------------------------------
            // 2. CAPSULE COLLIDER (Fizica)
            // ---------------------------------------------------------
            CapsuleCollider physicalCol = GetOrAddComponent<CapsuleCollider>(npc);
            physicalCol.center = new Vector3(0, 0.9f, 0); // Centrul la jumatatea inaltimii
            physicalCol.radius = 0.35f;
            physicalCol.height = 1.8f;
            

            // ---------------------------------------------------------
            // 3. ANIMATOR
            // ---------------------------------------------------------
            Animator anim = npc.GetComponent<Animator>();
            if (anim == null) anim = npc.AddComponent<Animator>();
            anim.applyRootMotion = false; // Important pentru NavMesh
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate; // Sa nu se opreasca cand nu te uiti la el

            // ---------------------------------------------------------
            // 4. PASSENGER AI (Scriptul tau de miscare)
            // ---------------------------------------------------------
            // *Nota: Trebuie sa tragi Waypoint-urile manual dupa, scriptul nu stie care sunt*
            PassengerAI aiScript = GetOrAddComponent<PassengerAI>(npc);
            // Putem seta variabile publice daca sunt expuse:
            // aiScript.minWaitTime = 2f; // Exemplu, daca variabilele sunt publice
            // aiScript.maxWaitTime = 8f;
            
            // ---------------------------------------------------------
            // 5. SPHERE COLLIDER (Trigger pentru Reactii/Quest)
            // ---------------------------------------------------------
            SphereCollider triggerCol = GetOrAddComponent<SphereCollider>(npc);
            triggerCol.isTrigger = true;  // BIFAT OBLIGATORIU
            triggerCol.radius = 1.0f;     // Raza de detectie
            triggerCol.center = new Vector3(0, 1.0f, 0);

            // ---------------------------------------------------------
            // 6. AUDIO SOURCE (Pentru voce)
            // ---------------------------------------------------------
            AudioSource audio = GetOrAddComponent<AudioSource>(npc);
            audio.spatialBlend = 1.0f; // 3D Sound (se aude din directia NPC-ului)
            audio.playOnAwake = false;

            // ---------------------------------------------------------
            // 7. NPC REACTION (Scriptul de "Hey! Watch out")
            // ---------------------------------------------------------
            GetOrAddComponent<NPCReaction>(npc);

            // ---------------------------------------------------------
            // 8. NPC VARIATIONS (Scriptul de variatie viteza/prioritate)
            // ---------------------------------------------------------
            GetOrAddComponent<NPCVariations>(npc);

            Debug.Log($"✅ NPC Configurat: {npc.name}");
        }
    }

    // Functie ajutatoare care nu adauga dubluri daca componenta exista deja
    static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }
}