using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace compilador
{
    class Sintatico
    {
        private LexScanner lexico;
        private Token token;
        private Dictionary<string, Symbol> tableSymbol = new Dictionary<string, Symbol>();
        private TokenEnum type;
        private TokenEnum typeExp;
        private List<string> C = new List<string>();
        private int s = -1;
        public Sintatico(string path)
        {
            lexico = new LexScanner(path);
        }

        private bool verifyToken(string term)
        {
            return (token != null && token.term.Equals(term));
        }

        public void analysis()
        {
            programa();
            if (token == null)
            {
                Console.WriteLine("Deu bom");
                File.WriteAllLines("C:/Users/wilso/OneDrive/Documentos/GitHub/compiladores2/output.txt", C.ToArray());
                
            }
            else
            {
                throw new RuntimeBinderInternalCompilerException($"Erro sintatico esperado fim de cadeia e foi encontrado: {token.term}");
            }
        }

        private void getToken()
        {
            token = lexico.nextToken();
            if (token.type == TokenEnum.COMMENT)
            {
                getToken();
            }
        }


        private void programa()
        {
            Console.WriteLine("programa");
            getToken();
            if (verifyToken("program"))
            {
                getToken();
                if (token.type == TokenEnum.IDENT)
                {
                    C = C.Append("INPP").ToList();
                    getToken();
                    corpo();
                    getToken();
                    if (!token.term.Equals("."))
                    {
                        throw new Exception($"Erro sintatico, esperado '.' e recebido {token}");
                    }

                    C = C.Append("PARA").ToList();
                    foreach (string content in C)
                    {
                        Console.WriteLine(content);
                    }
                    token = null;
                }
            }
        }

        private void corpo()
        {
            Console.WriteLine("corpo");
            dc();
            if (verifyToken("begin"))
            {
                getToken();
                comandos(); 
                if (!verifyToken("end"))
                {
                    throw new Exception($"Erro sintatico esperado 'end' e foi encontrado: {token.term}");
                } 
            }
            else
            {
                throw new Exception($"Erro sintatico esperado 'begin' e foi encontrado: {token.term}");

            }
        }

        private void dc()
        {
            Console.WriteLine("dc");
            if (!verifyToken("begin"))
            {
                dc_v();
                mais_dc();
            }
        }
        
        private void dc_v()
        {
            Console.WriteLine("dc_v");
            string tipo_dir = tipo_var();
            getToken();
            if (token.type == TokenEnum.ASSIGN)
            { 
                getToken();
                variaveis(tipo_dir);
            }
            else
            { 
                throw new Exception($"Erro sintatico, esperado ':' e foi encontrado: {token}");

            }
        }
        private void mais_dc()
        {
            Console.WriteLine("mais_dc");
            if (token.term.Equals(";"))
            {
                getToken();
                dc();
            }
            else
            {
                throw new Exception($"Erro sintatico esperado ';' recebido {token}");
            }
        }

        private string tipo_var()
        {
            Console.WriteLine("tipo_var");
            if (!(verifyToken("integer") || verifyToken("real")))
            {
                throw new Exception($"Erro sintatico, esperado 'INTEGER' ou 'REAL' e foi encontrado: {token}");

            }
            if(token.term.Equals("integer"))
            {
                type = TokenEnum.INTEGER;
                return "0";
            }
            else
            {
                type = TokenEnum.REAL;
                return "0.0";
            }
        }

        private void variaveis(string var_esq)
        {
            Console.WriteLine("variaveis");
            if (token.type == TokenEnum.IDENT)
            {
                if (tableSymbol.ContainsKey(token.term))
                {
                    throw new Exception($"Erro semantico, identificador ja encontrado: {token.term}");
                }
                else
                {
                    C = C.Append("ALME 1").ToList();
                    tableSymbol.Add(token.term, new Symbol(type, token.term, ++s));
                }
                getToken();
                mais_var(var_esq); 
            }

            else
            {
                throw new Exception($"Erro sintatico esperado 'IDENT' e foi encontrado: {token}");

            }
        }

        private void mais_var(string mais_var_esq)
        {
            Console.WriteLine("mais_var");
            if (token.term.Equals(","))
            {
                getToken();
                variaveis(mais_var_esq);
            }
        }
        
        private void comandos()
        {
            Console.WriteLine("comandos");
            comando();
            mais_comandos();
        }
        
        private void comando()
        {
            Console.WriteLine("comando");
            if (verifyToken("read") || verifyToken("write"))
            {
                string op = token.term;
                verif_parenteses(op);
            }
            else if (token.type == TokenEnum.IDENT)
            {
                verif_table_symbol();
                typeExp = tableSymbol[token.term].type;
                string value_ident = token.term;
                getToken();
                if (token.type == TokenEnum.ASSIGN)
                {
                    getToken();
                    expressao();
                    C = C.Append($"ARMZ {tableSymbol[value_ident].endRel}").ToList();
                }
            }
            else if (verifyToken("if"))
            {
                typeExp = TokenEnum.NULL; 
                getToken();
                condicao();
                if (verifyToken("then"))
                {
                    typeExp = TokenEnum.NULL;
                    getToken();
                    
                    int  DSVF_POS_ToReplace = C.Count;
                    C = C.Append("DSVF DSVF_POS_ToReplace").ToList();
                    
                    comandos();

                    int  DSVI_POS_ToReplace = C.Count;
                    C = C.Append("DSVI DSVI_POS_ToReplace").ToList();
                    
                    int posElse = C.Count;
                    pfalsa();

                    // sem else
                    if (C.Count == posElse)
                    {
                        C.RemoveAt(C.Count - 1);
                        C[DSVF_POS_ToReplace] = C[DSVF_POS_ToReplace]
                            .Replace("DSVF_POS_ToReplace", C.Count.ToString());
                    }
                    
                    // com else
                    if (C.Count > posElse)
                    {
                        C[DSVF_POS_ToReplace] = C[DSVF_POS_ToReplace]
                            .Replace("DSVF_POS_ToReplace", posElse.ToString());
                        C[DSVI_POS_ToReplace] = C[DSVI_POS_ToReplace]
                            .Replace("DSVI_POS_ToReplace", C.Count.ToString());
                    } 
                    
                    if (token.term.Equals("$"))
                    {
                        getToken();
                    }
                }
            }
            else if (verifyToken("while"))
            {
                getToken();
                int Condition_POS_ToReplace = C.Count;
                condicao();

                int DSVF_POS_ToReplace = C.Count;
                C = C.Append("DSVF DSVF_POS_ToReplace").ToList();
                if (verifyToken("do"))
                {
                    getToken();
                    comandos();

                    C = C.Append($"DSVI {Condition_POS_ToReplace}").ToList();

                    C[DSVF_POS_ToReplace] =
                        C[DSVF_POS_ToReplace].Replace("DSVF_POS_ToReplace", C.Count.ToString());
                    if (token.term.Equals("$"))
                    {
                        getToken();
                    }
                }
            }
            else
            {
                throw new Exception($"Erro sintatico esperado um comando e foi encontrado: {token}");

            }
        }

        private void mais_comandos()
        {
            Console.WriteLine("mais_comandos");
            if (token.term == ";")
            {
                getToken();
                if (!verifyToken("end"))
                {
                    comandos();
                }
            }
        }

        private void expressao()
        {
            
            Console.WriteLine("expressao");
            if (token.type == TokenEnum.IDENT)
            {
                verif_table_symbol();
                verif_type();
            }
            termo();
            outros_termos();
        }

        private string termo()
        {
            Console.WriteLine("termo");
            string op_minus = op_un();
            string fator_dir = fator();
            if (op_minus.Equals("-"))
            {
                string mais_fatores_dir = mais_fatores(fator_dir);
                return mais_fatores_dir;
            }
            else
            {
                string mais_fatores_dir = mais_fatores(fator_dir);
                return mais_fatores_dir;
            }
        }

        private string op_un()
        {
            
            Console.WriteLine("op_un");

            if (token.term.Equals("-"))
            {
                string op_un_dir = token.term;
                C = C.Append("INVE").ToList();
                getToken();
                return op_un_dir;
            }

            return "";
        }

        private string fator()
        {
            Console.WriteLine("fator");
            if (token.type is TokenEnum.IDENT)
            {
                verif_table_symbol();
                Token id = token;
                C = C.Append($"CRVL {tableSymbol[id.term].endRel}").ToList();
                getToken();
                return tableSymbol[id.term].value;
                
            }
            
            else if (token.type is TokenEnum.REAL or TokenEnum.INTEGER)
            {
                Token id = token;
                C = C.Append($"CRCT {id.term}").ToList();
                getToken();
                return id.term;
            }

            if (!token.term.Equals(";"))
            {
                getToken();
                if (token.term.Equals("("))
                {
                    getToken();
                    expressao();
                    if (token.term.Equals(")"))
                    {
                        getToken();
                    }
                }
            }

            return "";
        }

        private string mais_fatores(string fator_esq)
        {
            Console.WriteLine("mais_fatores");
            
            if (token.term is "*" or "/")
            {
                var op_mais_fatores = token.term;
                op_mul();
                var fator_dir = fator();
                C = op_mais_fatores == "*" ? C.Append("MULT").ToList() : C.Append("DIVI").ToList();
                var mais_fatores_dir = mais_fatores(fator_dir);
                return mais_fatores_dir;
            }
            else
            {
                return fator_esq;
            }
        }

        private void outros_termos()
        {
            Console.WriteLine("outros_termos");

            if (token.term is not ("+" or "-")) return;
            var op = token.term;
            op_ad();
            termo();
            C = op == "+" ? C.Append("SOMA").ToList() : C.Append("SUBT").ToList();
            outros_termos();
        }

        private void op_mul()
        {
            Console.WriteLine("op_mul");

            if (token.term is "*" or "/")
            {
                getToken();
            }
            else
            {
                throw new Exception($"Erro sintatico, esperado '*' ou '/' recebido {token}");

            }
        }

        private void op_ad()
        {
            Console.WriteLine("op_ad");

            if (token.term is "+" or "-")
            {
                getToken();
            }
            else
            {
                throw new Exception($"Erro sintatico, esperado '+' ou '-' recebido {token}");

            }
        }

        private void condicao()
        {
            expressao();
            var op_relational = relacao();
            expressao();
            C = op_relational switch
            {
                "=" => C.Append("CPIG").ToList(),
                "<>" => C.Append("CDES").ToList(),
                ">" => C.Append("CPMA").ToList(),
                "<" => C.Append("CPME").ToList(),
                ">=" => C.Append("CMAI").ToList(),
                "<=" => C.Append("CPMI").ToList(),
                _ => C
            };
        }

        private string relacao()
        {
            Console.WriteLine("relacao");
            if (token.type != TokenEnum.RELATIONAL) throw new Exception("Esperado um TOKEN relacional");
            var op = token.term;
            getToken();
            return op;

        }

        private void pfalsa()
        {
            Console.WriteLine("pfalsa");
            if (token.term.Equals("$")) return;
            if (!verifyToken("else")) return;
            getToken();
            comandos();
        }

        // Verifica se os parenteses estao sendo usados de maneira correta
        private void verif_parenteses(string op)
        {
            Console.WriteLine("verif_parenteses");
            getToken();
            if (token.term.Equals("("))
            {
                getToken();
                if (token.type == TokenEnum.IDENT)
                {
                    verif_table_symbol();
                    if (op == "read")
                    {
                        C = C.Append("LEIT").ToList();
                        C = C.Append($"ARMZ {tableSymbol[token.term].endRel}").ToList();
                    }
                    else
                    {
                        C = C.Append($"CRVL {tableSymbol[token.term].endRel}").ToList();
                        C = C.Append("IMPR").ToList();
                    }
                    getToken();
                    if (!(token.term.Equals(")")))
                    {
                        throw new Exception($"Erro sintatico, esperado ')'  e foi encontrado {token}");
                    }
                    getToken();
                }
                else
                {
                    throw new Exception($"Erro sintatico, esperado 'IDENT'  e foi encontrado {token}");
                }
            }
            else
            {
                throw new Exception($"Erro sintatico, esperado '('  e foi encontrado {token}");
            }
        }
        
        // Verifica se a variavel foi declarada
        private void verif_table_symbol()
        {
            if (!tableSymbol.ContainsKey(token.term))
            {
                throw new Exception($"Erro semantico, variavel '{token}' sendo usada e nao foi declarada"); 
            }
        }
        
        // Verifica se a variavel e a expressao sao do mesmo tipo
        private void verif_type()
        {
            Console.WriteLine("verify_type");
            if (typeExp != tableSymbol[token.term].type && typeExp != TokenEnum.NULL)
            {
                throw new Exception(
                    $"Variavel '{token.term}' do tipo {tableSymbol[token.term].type} sendo usada em expressao do tipo {typeExp}");
            }
        }
    }
}