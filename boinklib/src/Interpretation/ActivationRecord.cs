using System;
using System.Collections.Generic;

using Boink.Types;

namespace Boink.Interpretation
{
    /// <summary>
    /// Temporary memory of a procedure (program, function). Also contains information
    /// about the activation/call of a procedure.
    /// <para>Nesting level of the call to the procedure is the depth of the
    /// call e.g. a program's nesting level is 1. Parent record is the 
    /// caller of this procedure. A program procedure's parent record 
    /// is null and a function called in the global scope has the program
    /// for parent record. This way the interpreter can check the parent's 
    /// memory to find some variables that were not declared in its scope.</para>
    /// </summary>
    /// <para>
    /// 
    /// *Example:*
    /// <code>
    ///  fn increment(int a)
    ///      int c = a + 1
    ///  ;
    ///       
    ///  increment(1)
    /// </code>
    /// ------- END OF FUNCTION increment -------
    /// CALL STACK
    /// 2 increment   ---> Activation record for increment call in line 5.
    ///     a : 1
    ///     c : 2
    /// 1 test.boink  ---> Activation record for the program.
    ///     increment : function_
    /// </para>
    public class ActivationRecord
    {
        public string Name { get; private set; }

        public int NestingLevel { get; private set; }

        public ActivationRecord ParentRecord { get; private set; }

        public function_ Owner { get; private set; }

        public Dictionary<string, obj_> Members { get; private set; }

        /// <summary>Construct an ActivationRecord object.</summary>
        /// <param name="name">Name of the procedure that is called.</param>
        /// <param name="nestingLevel">Nesting level of the activation.</param>
        /// <param name="parentRecord">Caller of the activation.</param>
        /// <param name="owner">Owner of the record, a function. Defaults to null.</param>
        public ActivationRecord(string name, int nestingLevel, ActivationRecord parentRecord, function_ owner = null)
        {
            Name = name;
            NestingLevel = nestingLevel;
            ParentRecord = parentRecord;
            Owner = owner;
            Members = new Dictionary<string, obj_>();
        }

        public void SetVar(string name, obj_ value)
        {
            Members[name] = value;
        }

        public void DefineVar(string name, obj_ value)
        {
            Members.Add(name, value);
        }

        public obj_ GetVar(string name)
        {
            obj_ objInCurrentScope = null;
            Members.TryGetValue(name, out objInCurrentScope);

            if (objInCurrentScope == null)
            {
                if (ParentRecord != null)
                    return ParentRecord.GetVar(name);
                
                throw new Exception("There is either an error in the semantic analyzer or the definition of variables.");
            }
            else
                return objInCurrentScope;
        }

        public override string ToString()
        {
            List<string> lines = new List<string> { $"{NestingLevel} {Name}" };
            lines.Add("{");
            foreach (KeyValuePair<string, obj_> kv in Members)
                lines.Add($"   {kv.Key}: {kv.Value.ToString()}");

            lines.Add("}");
            string s = "";
            foreach (string l in lines)
                s += l + "\n";

            return s;
        }

    }

}
