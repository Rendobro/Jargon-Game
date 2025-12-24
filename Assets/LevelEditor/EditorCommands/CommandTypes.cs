using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandTypes
{
    public class MoveCommand : IEditorCommand
    {
        private readonly List<ObjectData> objs = new();
        private readonly List<Vector3> prevPosArr = new();
        private readonly List<Vector3> targetPosArr = new();
        public MoveCommand(List<ObjectData> objs)
        {
            this.objs = new List<ObjectData>(objs);
            for (int i = 0; i < objs.Count; i++)
                prevPosArr.Add(objs[i].transform.position);
        }
        public void Execute()
        {
            if (targetPosArr == null) throw new System.ArgumentNullException();
            for (int i = 0; i < objs.Count; i++)
                objs[i].transform.position = targetPosArr[i];
        }
        public void Undo()
        {
            for (int i = 0; i < objs.Count; i++)
                objs[i].transform.position = prevPosArr[i];
        }
        public void SetFinalState()
        {
            targetPosArr.Clear();
            for (int i = 0; i < objs.Count; i++)
                targetPosArr.Add(objs[i].transform.position);
        }
    }

    public class RotateCommand : IEditorCommand
    {
        private struct RotateState
        {
            public Quaternion rot;
            public Vector3 pos;
            public RotateState(Quaternion q, Vector3 p)
            {
                rot = q;
                pos = p;
            }
        }
        private readonly List<ObjectData> objs = new();
        private readonly List<RotateState> prevRotArr = new();
        private readonly List<RotateState> targetRotArr = new();
        public RotateCommand(List<ObjectData> objs)
        {
            this.objs = new List<ObjectData>(objs);
            for (int i = 0; i < objs.Count; i++)
                prevRotArr.Add(new RotateState(objs[i].transform.rotation, objs[i].transform.position));
        }
        public void Execute()
        {
            if (targetRotArr == null) throw new System.ArgumentNullException();
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].transform.rotation = targetRotArr[i].rot;
                objs[i].transform.position = targetRotArr[i].pos;
            }
        }
        public void Undo()
        {
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].transform.rotation = prevRotArr[i].rot;
                objs[i].transform.position = prevRotArr[i].pos;
            }
        }
        public void SetFinalState()
        {
            targetRotArr.Clear();
            for (int i = 0; i < objs.Count; i++)
                targetRotArr.Add(new RotateState(objs[i].transform.rotation, objs[i].transform.position));
        }
    }

    public class ScaleCommand : IEditorCommand
    {
        private struct ScaleState
        {
            public Vector3 scale;
            public Vector3 pos;
            public ScaleState(Vector3 s, Vector3 p)
            {
                scale = s;
                pos = p;
            }
        }
        private readonly List<ObjectData> objs = new();
        private readonly List<ScaleState> prevScaleArr = new();
        private readonly List<ScaleState> targetScaleArr = new();
        public ScaleCommand(List<ObjectData> objs)
        {
            this.objs = new List<ObjectData>(objs);
            for (int i = 0; i < objs.Count; i++)
                prevScaleArr.Add(new ScaleState(objs[i].transform.localScale,objs[i].transform.localPosition));
        }
        public void Execute()
        {
            if (targetScaleArr == null) throw new System.ArgumentNullException();
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].transform.localScale = targetScaleArr[i].scale;
                objs[i].transform.localPosition = targetScaleArr[i].pos;
            }
        }
        public void Undo()
        {
            for (int i = 0; i < objs.Count; i++)
             {
                objs[i].transform.localScale = prevScaleArr[i].scale;
                objs[i].transform.localPosition = prevScaleArr[i].pos;
            }
        }
        public void SetFinalState()
        {
            targetScaleArr.Clear();
            for (int i = 0; i < objs.Count; i++)
                targetScaleArr.Add(new ScaleState(objs[i].transform.localScale,objs[i].transform.localPosition));
        }
    }
}