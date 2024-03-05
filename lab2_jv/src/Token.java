public class Token {
    public enum Type {
        NUM, OP, LPAR, RPAR, SEMICOLON, DIV, MOD, ID, EOF
    }

    private final Type type;
    private final String value;

    public Token(Type type, String value) {
        this.type = type;
        this.value = value;
    }

    public Type getType() {
        return type;
    }

    public String getValue() {
        return value;
    }

    @Override
    public String toString() {
        switch (type) {
            case LPAR:
            case RPAR:
            case SEMICOLON:
            case DIV:
            case MOD:
                return type.toString();
            default:
                return type + ":" + (value != null ? value : "");
        }
    }
}
