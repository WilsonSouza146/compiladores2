namespace compilador
{
    class Symbol
    {
        public string value { get; set; }
        public TokenEnum type { get; set; }

        public int endRel { get; set; }

        public Symbol(TokenEnum type, string value, int endRel)
        {
            this.value = value;
            this.endRel = endRel;
            this.type = type;
        }
    }
}