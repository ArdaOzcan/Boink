using System;
using System.Collections.Generic;

using Boink.Analysis.Tokenization;
using Boink.Errors;

namespace Boink.Types
{
    public static class OperationTypes
    {
        public static readonly Dictionary<TokenType, Operation> UnaryOperationTokens = new Dictionary<TokenType, Operation>
        {
            { TokenType.Plus, Operation.Positive },
            { TokenType.Minus, Operation.Negative },
        };

        public static readonly Dictionary<Type, HashSet<Operation>> UnaryOperationTypes = new Dictionary<Type, HashSet<Operation>>
        {
            {
                typeof(IntType), new HashSet<Operation>
                {
                    { Operation.Positive },
                    { Operation.Negative }
                }
            },
            {
                typeof(FloatType), new HashSet<Operation>
                {
                    { Operation.Positive },
                    { Operation.Negative }
                }
            },
            {
                typeof(DoubleType), new HashSet<Operation>
                {
                    { Operation.Positive },
                    { Operation.Negative }
                }
            },
        };

        public static readonly Dictionary<TokenType, Operation> BinaryOperationTokens = new Dictionary<TokenType, Operation>
        {
            { TokenType.Plus, Operation.Add },
            { TokenType.Minus, Operation.Subtract },
            { TokenType.Star, Operation.Multiply },
            { TokenType.Slash, Operation.Divide },
            { TokenType.AmpersandAmpersand, Operation.And },
            { TokenType.PipePipe, Operation.Or },
            { TokenType.Greater, Operation.Greater },
            { TokenType.GreaterEquals, Operation.GreaterEquals },
            { TokenType.Less, Operation.Less },
            { TokenType.LessEquals, Operation.LessEquals },
            { TokenType.EqualsEquals, Operation.EqualsEquals }
        };

        ///<summary>Temporary dictionary to hold information about resulting types of operations.
        /// Just there to make it work, will be replaced with something more sustainable and less _horrendous_.</summary>
        private static readonly Dictionary<Type, Dictionary<Operation, Dictionary<Type, Type>>> BinaryOperationResultTypes = new Dictionary<Type, Dictionary<Operation, Dictionary<Type, Type>>>
        {
            {
                typeof(IntType), new Dictionary<Operation, Dictionary<Type, Type>>
                {
                    {
                        Operation.Add, new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(IntType) },
                            { typeof(FloatType), typeof(FloatType)}, 
                            { typeof(DoubleType), typeof(DoubleType)}
                    }
                    },
                    {
                        Operation.Subtract, new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(IntType) },
                            { typeof(FloatType), typeof(FloatType)}, 
                            { typeof(DoubleType), typeof(DoubleType)}
                    }
                    },
                    {
                        Operation.Multiply,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(IntType) },
                            { typeof(FloatType), typeof(FloatType)}, 
                            { typeof(DoubleType), typeof(DoubleType)}
                        }
                    },
                    {
                        Operation.Divide,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(DoubleType) },
                            { typeof(FloatType), typeof(FloatType)}, 
                            { typeof(DoubleType), typeof(DoubleType)}
                        }
                    },
                    {
                        Operation.Greater,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType)}, 
                            { typeof(DoubleType), typeof(BoolType)}
                        }
                    },
                    {
                        Operation.GreaterEquals,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType)}, 
                            { typeof(DoubleType), typeof(BoolType)}
                        }
                    },
                    {
                        Operation.Less,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType)}, 
                            { typeof(DoubleType), typeof(BoolType)}
                        }
                    },
                    {
                        Operation.LessEquals,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType)}, 
                            { typeof(DoubleType), typeof(BoolType)}
                        }
                    },
                    {
                        Operation.EqualsEquals,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType)}, 
                            { typeof(DoubleType), typeof(BoolType)}
                        }
                    }
                }
            },
            {
                typeof(FloatType),new Dictionary<Operation, Dictionary<Type, Type>>
                {
                    {
                        Operation.Add, new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(FloatType) },
                            { typeof(FloatType), typeof(FloatType) },
                            { typeof(DoubleType), typeof(DoubleType) }
                        }
                    },
                    {
                        Operation.Subtract,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(FloatType) },
                            { typeof(FloatType), typeof(FloatType) },
                            { typeof(DoubleType), typeof(DoubleType) }
                        }
                    },
                    {
                        Operation.Multiply,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(FloatType) },
                            { typeof(FloatType), typeof(FloatType) },
                            { typeof(DoubleType), typeof(DoubleType) }
                        }
                    },
                    {
                        Operation.Divide,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(FloatType) },
                            { typeof(FloatType), typeof(FloatType) },
                            { typeof(DoubleType), typeof(DoubleType) }
                        }
                    },
                    {
                        Operation.Greater,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    },
                    {
                        Operation.GreaterEquals,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    },
                    {
                        Operation.Less,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    },
                    {
                        Operation.LessEquals,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    },
                    {
                        Operation.EqualsEquals,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    }
                }
            },
            {
                typeof(BoolType),new Dictionary<Operation, Dictionary<Type, Type>>
                {
                    {
                        Operation.And, new Dictionary<Type, Type>
                        {
                            { typeof(BoolType), typeof(BoolType) },
                        }
                    },
                    {
                        Operation.Or,new Dictionary<Type, Type>
                        {
                            { typeof(BoolType), typeof(BoolType) },
                        }
                    },
                    {
                        Operation.EqualsEquals,new Dictionary<Type, Type>
                        {
                            { typeof(BoolType), typeof(BoolType) },
                        }
                    }
                }
            },
            {
                typeof(DoubleType), new Dictionary<Operation, Dictionary<Type, Type>>
                {
                    {
                        Operation.Add, new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(DoubleType) },
                            { typeof(DoubleType), typeof(DoubleType) }
                        }
                    },
                    {
                        Operation.Subtract,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(DoubleType) },
                            { typeof(DoubleType), typeof(DoubleType) }
                        }
                    },
                    {
                        Operation.Multiply,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(DoubleType) },
                            { typeof(DoubleType), typeof(DoubleType) }
                        }
                    },
                    {
                        Operation.Divide,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(DoubleType) },
                            { typeof(DoubleType), typeof(DoubleType) }
                        }
                    },
                    {
                        Operation.Greater,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    },
                    {
                        Operation.GreaterEquals,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    },
                    {
                        Operation.Less,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    },
                    {
                        Operation.LessEquals,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    },
                    {
                        Operation.EqualsEquals,new Dictionary<Type, Type>
                        {
                            { typeof(IntType), typeof(BoolType) },
                            { typeof(FloatType), typeof(BoolType) },
                            { typeof(DoubleType), typeof(BoolType) }
                        }
                    }
                }
            },
            {
                typeof(StringType), new Dictionary<Operation, Dictionary<Type, Type>>
                {
                    {
                        Operation.Add,new Dictionary<Type, Type>
                        {
                            { typeof(StringType), typeof(StringType) }
                        }
                    }
                }
            }
        };

        public static Operation GetBinaryOperationByTokenType(TokenType tokenType)
        {
            Operation op = Operation.Null;
            BinaryOperationTokens.TryGetValue(tokenType, out op);
            return op;
        }

        public static Operation GetUnaryOperationByTokenType(TokenType tokenType)
        {
            Operation op = Operation.Null;
            UnaryOperationTokens.TryGetValue(tokenType, out op);
            return op;
        }


        public static Type GetBinaryResultType(Type thisType, TokenType tokenType, Type otherType, int pos) =>
            GetBinaryResultType(thisType, GetBinaryOperationByTokenType(tokenType), otherType, pos);

        public static Type GetBinaryResultType(Type thisType, Operation operation, Type otherType, int pos)
        {
            if (thisType is null) return null;

            Dictionary<Operation, Dictionary<Type, Type>> opDict;
            if (!BinaryOperationResultTypes.TryGetValue(thisType, out opDict))
            {
                // ErrorHandler.Throw(new UnsupportedOperationError($"Type {thisType.Name} doesn't support binary operations", pos));
                return null;
            }

            Dictionary<Type, Type> typeDict;
            if (!opDict.TryGetValue(operation, out typeDict))
            {
                // ErrorHandler.Throw(new UnsupportedOperationError($"Type {thisType.Name} doesn't support {operation.ToString()}", pos));
                return null;
            }

            Type resultType;
            if (!typeDict.TryGetValue(otherType, out resultType))
            {
                // ErrorHandler.Throw(new UnsupportedOperationError($"Type {thisType.Name} doesn't support {operation.ToString()} with type {otherType.Name}", pos));
                return null;
            }

            return resultType;
        }

        public static bool TypeSupportsUnaryOperation(Type type, TokenType tokenType, int pos) => 
            TypeSupportsUnaryOperation(type, GetUnaryOperationByTokenType(tokenType), pos);

        public static bool TypeSupportsUnaryOperation(Type type, Operation operation, int pos)
        {
            if (!UnaryOperationTypes.ContainsKey(type)) return false;
            if (!UnaryOperationTypes[type].Contains(operation)) return false;
            return true;
        }
    }

}
