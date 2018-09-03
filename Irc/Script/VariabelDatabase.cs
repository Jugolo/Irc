using System;
using System.Collections.Generic;
using torrent.Script.Error;
using torrent.Script.Stack;
using torrent.Script.Values;

namespace torrent.Script
{
    public class VariabelDatabase
    {
        private List<VariabelStack> stack = new List<VariabelStack>();

        public VariabelDatabase()
        {
            this.stack.Add(new VariabelStack());
        }

        public int Size()
        {
            return this.stack.Count;
        }

        public void Push(VariabelStack stack)
        {
            this.stack.Add(stack);
        }

        public void Pop()
        {
            if (this.Size() == 1)
                throw new ScriptRuntimeException("Cant empty variabel database stack");
            this.stack.Remove(this.stack[this.stack.Count - 1]);
        }

        public VariabelStack Get(int i)
        {
            return this.stack[i];
        }
    }
}