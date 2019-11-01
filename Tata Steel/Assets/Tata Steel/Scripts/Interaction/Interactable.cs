using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        OnPress,
        OnHold,
        OnRelease
    }

    //Dont use mixed, since it doesnt work well in this scenario, just use different instances
    [SerializeField] private OVRInput.RawButton buttonToPress;
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private UnityEvent onStartInteraction;
    [SerializeField] private UnityEvent whileInteracting;
    [SerializeField] private UnityEvent onEndInteraction;
    [SerializeField] private bool continueOutOfRange;

    protected MeshRenderer[] highlightRenderers;
    protected MeshRenderer[] existingRenderers;
    protected GameObject highlightHolder;
    protected SkinnedMeshRenderer[] highlightSkinnedRenderers;
    protected SkinnedMeshRenderer[] existingSkinnedRenderers;
    //protected static Material highlightMat;
    [Tooltip("An array of child gameObjects to not render a highlight for. Things like transparent parts, vfx, etc.")]
    public GameObject[] hideHighlight;

    //[SerializeField] private Material material;
    [SerializeField] private Material outlineMaterial;

    [SerializeField] private List<Renderer> renderers;

    private List<Material[]> originalMaterials = new List<Material[]>();
    private bool canInteract = false;

    public bool isInteracting { get; private set; }

    public List<Transform> hands { get; private set; } = new List<Transform>();

    public Transform closestHand { get; private set; } = null;

    public OVRInput.Controller controller { get; private set; } = OVRInput.Controller.None;

    private const float TIME_BETWEEN_HAND_CHECKS = 0.1f;

    private void Start()
    {
        if (renderers.Count != 0)
        {
            foreach (Renderer r in renderers)
            {
                originalMaterials.Add(r.materials);
            }
        }

        StartCoroutine(CheckForClosestHand(TIME_BETWEEN_HAND_CHECKS));
    }

    private bool InputValid()
    {
        List<string> buttons = new List<string>();
        char[] b = buttonToPress.ToString().ToCharArray();
        int lastIndex = 0;

        for (int i = 0; i < b.Length; i++)
        {
            if (b[i] == ',')
            {
                string s = "";
                for (int j = lastIndex; j < i; j++)
                {
                    s += b[j];
                }

                buttons.Add(s);

                lastIndex = i + 2;
            }
        }

        string last = "";
        for (int i = lastIndex; i < b.Length; i++)
        {
            last += b[i];
        }
        buttons.Add(last);

        List<string> leftButtons = new List<string>();
        List<string> rightButtons = new List<string>();

        //Since the RawButtons are divided into LIndexTrigger, RIndexTrigger, etc to show what controller is currently being used, 
        //we can use the prefix to get the side at which it is being pressed. 
        //This way we can limit the input to the controller which is closest to the object. 
        for (int i = 0; i < buttons.Count; i++)
            if (buttons[i].Substring(0, 1) == "L")
                leftButtons.Add(buttons[i]);
            else if (buttons[i] == "X" || buttons[i] == "Y")
                leftButtons.Add(buttons[i]);
            else if (buttons[i].Substring(0, 1) == "R")
                rightButtons.Add(buttons[i]);            
            else if (buttons[i] == "A" || buttons[i] == "B")
                rightButtons.Add(buttons[i]);

        if (controller == OVRInput.Controller.LTouch)
            return CheckForInputFromButtonNames(leftButtons);
        else if (controller == OVRInput.Controller.RTouch)
            return CheckForInputFromButtonNames(rightButtons);

        return false;   
    }

    private bool CheckForInputFromButtonNames(List<string> buttons)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            switch (interactionType)
            {
                case InteractionType.OnHold:
                    if (OVRInput.Get(GetButtonByName(buttons[i])))
                        return true;
                    break;
                case InteractionType.OnPress:
                    if (OVRInput.GetDown(GetButtonByName(buttons[i])))
                        return true;
                    break;
                case InteractionType.OnRelease:
                    if (OVRInput.GetUp(GetButtonByName(buttons[i])))
                        return true;
                    break;
            }
        }

        return false;
    }

    private OVRInput.RawButton GetButtonByName(string name)
    {
        Array t = Enum.GetValues(typeof(OVRInput.RawButton));

        for (int i = 0; i < t.Length; i++)
        {
            if (t.GetValue(i).ToString().Equals(name))
            {
                return (OVRInput.RawButton) t.GetValue(i);
            }
        }

        return OVRInput.RawButton.None;
    }

    private void Update()
    {
        UpdateHighlightRenderers();
    }

    private void FixedUpdate()
    {
        if (canInteract)
        {
            if (InputValid())
            {
                if (!isInteracting)
                {
                    onStartInteraction.Invoke();
                }

                //Make sure to not call while interacting in the first frame of interaction.
                if (isInteracting && interactionType == InteractionType.OnHold)
                {
                    whileInteracting.Invoke();
                }

                isInteracting = true;
            }
            else if (isInteracting)
            {
                isInteracting = false;
                onEndInteraction.Invoke();
            }
        }
        else if (isInteracting)
        {
            if (interactionType == InteractionType.OnHold)
            {
                whileInteracting.Invoke();
            }

            if (!continueOutOfRange || !OVRInput.Get(buttonToPress))
            {
                isInteracting = false;
                onEndInteraction.Invoke();
            }
        }
    }

    //this will have to be replaced to a hand(s) script keeping track of objects instead of the other way around.
    //if there are a lot of objects this will hit the performance quite a bit the way it is right now,
    //even though it only gets called 10 times a second.
    //This also does not account for different items being in reach of the hand which might become a problem in the future.
    //We'll have to see though. I'm too depressed to change this right now.
    private IEnumerator CheckForClosestHand(float time)
    {
        while (true)
        {
            if (hands == null || hands.Count == 0 || (!isInteracting && !canInteract))
            {
                closestHand = null;
            }

            if (hands != null && hands.Count > 0 && !isInteracting)
            {
                float closestDistance = float.MaxValue;

                foreach (Transform hand in hands)
                {
                    float distance = Vector3.Distance(transform.position, hand.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestHand = hand;
                    }
                }
            }

            if (closestHand != null)
            {
                foreach (Renderer r in renderers)
                    r.material = outlineMaterial;
                canInteract = true;

                controller = closestHand.name == "LeftHand" ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
            }
            else
            {
                for (int i = 0; i < renderers.Count; i++)
                    renderers[i].materials = originalMaterials[i];
                canInteract = false;

                //controller = OVRInput.Controller.None;
            }

            yield return new WaitForSeconds(time);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerController"))
        {
            if (!hands.Contains(other.transform))
                hands.Add(other.transform);

            CreateHighlightRenderers();
            UpdateHighlightRenderers();

            StopAllCoroutines();
            StartCoroutine(CheckForClosestHand(0.1f));
        }
    }  

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerController"))
        {
            if (hands.Contains(other.transform))
                hands.Remove(other.transform);

            StopAllCoroutines();
            StartCoroutine(CheckForClosestHand(0.1f));
        }
    }

    protected virtual bool ShouldIgnoreHighlight(Component component)
    {
        return ShouldIgnore(component.gameObject);
    }

    protected virtual bool ShouldIgnore(GameObject check)
    {
        for (int ignoreIndex = 0; ignoreIndex < hideHighlight.Length; ignoreIndex++)
        {
            if (check == hideHighlight[ignoreIndex])
                return true;
        }

        return false;
    }

    protected virtual void CreateHighlightRenderers()
    {
        existingSkinnedRenderers = this.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        highlightHolder = new GameObject("Highlighter");
        highlightSkinnedRenderers = new SkinnedMeshRenderer[existingSkinnedRenderers.Length];

        for (int skinnedIndex = 0; skinnedIndex < existingSkinnedRenderers.Length; skinnedIndex++)
        {
            SkinnedMeshRenderer existingSkinned = existingSkinnedRenderers[skinnedIndex];

            if (ShouldIgnoreHighlight(existingSkinned))
                continue;

            GameObject newSkinnedHolder = new GameObject("SkinnedHolder");
            newSkinnedHolder.transform.parent = highlightHolder.transform;
            SkinnedMeshRenderer newSkinned = newSkinnedHolder.AddComponent<SkinnedMeshRenderer>();
            Material[] materials = new Material[existingSkinned.sharedMaterials.Length];
            for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                materials[materialIndex] = outlineMaterial;
            }

            newSkinned.sharedMaterials = materials;
            newSkinned.sharedMesh = existingSkinned.sharedMesh;
            newSkinned.rootBone = existingSkinned.rootBone;
            newSkinned.updateWhenOffscreen = existingSkinned.updateWhenOffscreen;
            newSkinned.bones = existingSkinned.bones;

            highlightSkinnedRenderers[skinnedIndex] = newSkinned;
        }

        MeshFilter[] existingFilters = this.GetComponentsInChildren<MeshFilter>(true);
        existingRenderers = new MeshRenderer[existingFilters.Length];
        highlightRenderers = new MeshRenderer[existingFilters.Length];

        for (int filterIndex = 0; filterIndex < existingFilters.Length; filterIndex++)
        {
            MeshFilter existingFilter = existingFilters[filterIndex];
            MeshRenderer existingRenderer = existingFilter.GetComponent<MeshRenderer>();

            if (existingFilter == null || existingRenderer == null || ShouldIgnoreHighlight(existingFilter))
                continue;

            GameObject newFilterHolder = new GameObject("FilterHolder");
            newFilterHolder.transform.parent = highlightHolder.transform;
            MeshFilter newFilter = newFilterHolder.AddComponent<MeshFilter>();
            newFilter.sharedMesh = existingFilter.sharedMesh;
            MeshRenderer newRenderer = newFilterHolder.AddComponent<MeshRenderer>();

            Material[] materials = new Material[existingRenderer.sharedMaterials.Length];
            for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                materials[materialIndex] = outlineMaterial;
            }
            newRenderer.sharedMaterials = materials;

            highlightRenderers[filterIndex] = newRenderer;
            existingRenderers[filterIndex] = existingRenderer;
        }
    }

    protected virtual void UpdateHighlightRenderers()
    {
        if (highlightHolder == null)
            return;

        for (int skinnedIndex = 0; skinnedIndex < existingSkinnedRenderers.Length; skinnedIndex++)
        {
            SkinnedMeshRenderer existingSkinned = existingSkinnedRenderers[skinnedIndex];
            SkinnedMeshRenderer highlightSkinned = highlightSkinnedRenderers[skinnedIndex];

            if (existingSkinned != null && highlightSkinned != null && !isInteracting)
            {
                highlightSkinned.transform.position = existingSkinned.transform.position;
                highlightSkinned.transform.rotation = existingSkinned.transform.rotation;
                highlightSkinned.transform.localScale = existingSkinned.transform.lossyScale;
                highlightSkinned.localBounds = existingSkinned.localBounds;
                highlightSkinned.enabled = closestHand && existingSkinned.enabled && existingSkinned.gameObject.activeInHierarchy;

                int blendShapeCount = existingSkinned.sharedMesh.blendShapeCount;
                for (int blendShapeIndex = 0; blendShapeIndex < blendShapeCount; blendShapeIndex++)
                {
                    highlightSkinned.SetBlendShapeWeight(blendShapeIndex, existingSkinned.GetBlendShapeWeight(blendShapeIndex));
                }
            }
            else if (highlightSkinned != null)
                highlightSkinned.enabled = false;

        }

        for (int rendererIndex = 0; rendererIndex < highlightRenderers.Length; rendererIndex++)
        {
            MeshRenderer existingRenderer = existingRenderers[rendererIndex];
            MeshRenderer highlightRenderer = highlightRenderers[rendererIndex];

            if (existingRenderer != null && highlightRenderer != null && isInteracting == false)
            {
                highlightRenderer.transform.position = existingRenderer.transform.position;
                highlightRenderer.transform.rotation = existingRenderer.transform.rotation;
                highlightRenderer.transform.localScale = existingRenderer.transform.lossyScale;
                highlightRenderer.enabled = closestHand && existingRenderer.enabled && existingRenderer.gameObject.activeInHierarchy;
            }
            else if (highlightRenderer != null)
                highlightRenderer.enabled = false;
        }
    }
}
