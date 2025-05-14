using System;
using TMPro;
using UnityEngine;

namespace AndreStuff
{
    public class EquipInteractable : MonoBehaviour
    {

        [SerializeField] private string hoveredText = "Press <b>E</b> to equip!";
        private bool _hovered = false;
        private Rigidbody _rb;
        private Collider _boxCollider;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<Collider>();
        }

        private void Start()
        {
            //transform.Find("ObjectCanvas").Find("Text").GetComponent<TMP_Text>().text = hoveredText;
            //UpdateState();
        }

        private void OnMouseEnter()
        {
            _hovered = true;
            UpdateState();
        }

        private void OnMouseExit()
        {
            _hovered = false;
            UpdateState();

        }

        private void UpdateState()
        {
            //transform.Find("ObjectCanvas").gameObject.SetActive(_hovered);
        }

        public void Equipped()
        {
            if (_rb != null) _rb.isKinematic = true;
            if (_boxCollider != null) _boxCollider.enabled = false;
        }

        public void Unequipped()
        {
            if (_rb != null) _rb.isKinematic = false;
            if (_boxCollider != null) _boxCollider.enabled = true;
        }
    }
}