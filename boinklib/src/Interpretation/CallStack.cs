using System.Collections.Generic;

namespace Boink.Interpretation
{

    /// <summary>
    /// Simple data structure to keep track of activation records.
    /// </summary>
    public class CallStack
    {
        private List<ActivationRecord> records;

        /// <summary>
        /// Return the last element of the call stack.
        /// </summary>
        /// <returns>The last activation record.</returns>
        public ActivationRecord Peek => records[records.Count - 1];

        /// <summary>
        /// Construct a CallStack object.
        /// </summary>
        public CallStack()
        {
            records = new List<ActivationRecord>();
        }

        /// <summary>
        /// Add a activation record to the call stack.
        /// </summary>
        /// <param name="ar">Activation record to add.</param>
        public void Push(ActivationRecord ar) => records.Add(ar);

        /// <summary>
        /// Remove a record from the end of the call stack.
        /// </summary>
        /// <returns>Removed record.</returns>
        public ActivationRecord Pop()
        {
            ActivationRecord last = records[records.Count - 1];
            records.RemoveAt(records.Count - 1);
            return last;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = records.Count; i-- > 0;)
            {
                ///do something
                s += records[i].ToString() + "\n";
            }
            s = $"CALL STACK\n{s}\n";
            return s;
        }
    }

}
