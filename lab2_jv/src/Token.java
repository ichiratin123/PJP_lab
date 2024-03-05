public class Token {
    enum Type {
        NUM, ID, OP, LPAR, RPAR, DIV, MOD, SEMICOLON, EOF
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
        return type + (value.isEmpty() ? "" : (":" + value));
    }
}
