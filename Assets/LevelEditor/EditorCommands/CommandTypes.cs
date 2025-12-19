using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandTypes
{
    public class MoveCommand : IEditorCommand
    {
        readonly ObjectData obj;
        readonly Vector3 prevPos;
        Vector3 targetPos;
        public MoveCommand(ObjectData obj)
        {
            this.obj = obj;
            prevPos = obj.transform.position;
        }

        public void Execute()
        {
            if (targetPos == null) throw new System.ArgumentNullException();
            obj.transform.position = targetPos;
        }

        public void Undo()
        {
            obj.transform.position = prevPos;
        }
        public void SetFinalState()
        {
            targetPos = obj.transform.position;
        }
    }

    public class RotateCommand : IEditorCommand
    {
        readonly ObjectData obj;
        readonly Quaternion prevRot;
        Quaternion targetRot;
        public RotateCommand(ObjectData obj)
        {
            this.obj = obj;
            prevRot = obj.transform.localRotation;
        }

        public void Execute()
        {
            if (targetRot == null) throw new System.ArgumentNullException();
            obj.transform.localRotation = targetRot;
        }

        public void Undo()
        {
            obj.transform.localRotation = prevRot;
        }
        public void SetFinalState()
        {
            targetRot = obj.transform.localRotation;
        }
    }

    public class ScaleCommand : IEditorCommand
    {
        readonly ObjectData obj;
        readonly Vector3 prevScale;
        Vector3 targetScale;
        public ScaleCommand(ObjectData obj)
        {
            this.obj = obj;
            prevScale = obj.transform.localScale;
        }

        public void Execute()
        {
            if (targetScale == null) throw new System.ArgumentNullException();
            obj.transform.localScale = targetScale;
        }

        public void Undo()
        {
            obj.transform.localScale = prevScale;
        }

        public void SetFinalState()
        {
            targetScale = obj.transform.localScale;
        }
    }
}