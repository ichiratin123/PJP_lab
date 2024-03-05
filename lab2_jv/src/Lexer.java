import java.util.ArrayList;
import java.util.List;

public class Lexer {
    private final String input;
    private final List<Token> tokens = new ArrayList<>();
    private int position = 0;

    public Lexer(String input) {
        this.input = input;
    }

    public List<Token> tokenize() {
        while (position < input.length()) {
            char current = peek();

            if (Character.isWhitespace(current)) {
                position++; // skip whitespaces
            } else if (Character.isDigit(current) || (current == '-' && Character.isDigit(peekAhead()))) {
                tokens.add(new Token(Token.Type.NUM, consumeNumber()));
            } else if (Character.isLetter(current)) {
                tokens.add(new Token(Token.Type.ID, consumeIdentifier()));
            } else {
                switch (current) {
                    case '+': case '-': case '*':
                        tokens.add(new Token(Token.Type.OP, String.valueOf(current)));
                        position++;
                        break;
                    case '/':
                        if (peekAhead() == '/') {
                            consumeComment();
                        } else {
                            tokens.add(new Token(Token.Type.DIV, String.valueOf(current)));
                            position++;
                        }
                        break;
                    case '%':
                        tokens.add(new Token(Token.Type.MOD, String.valueOf(current)));
                        position++;
                        break;
                    case '(':
                        tokens.add(new Token(Token.Type.LPAR, String.valueOf(current)));
                        position++;
                        break;
                    case ')':
                        tokens.add(new Token(Token.Type.RPAR, String.valueOf(current)));
                        position++;
                        break;
                    case ';':
                        tokens.add(new Token(Token.Type.SEMICOLON, String.valueOf(current)));
                        position++;
                        break;
                    default:
                        // Handle unexpected characters
                        position++;
                        break;
                }
            }
        }
        tokens.add(new Token(Token.Type.EOF, ""));
        return tokens;
    }

    private char peek() {
        return input.charAt(position);
    }

    private char peekAhead() {
        return position + 1 < input.length() ? input.charAt(position + 1) : '\0';
    }

    private void consumeComment() {
        while (position < input.length() && input.charAt(position) != '\n') {
            position++;
        }
    }

    private String consumeNumber() {
        int startIndex = position;
        if (input.charAt(position) == '-') {
            position++;
        }
        while (position < input.length() && Character.isDigit(input.charAt(position))) {
            position++;
        }
        return input.substring(startIndex, position);
    }

    private String consumeIdentifier() {
        int startIndex = position;
        while (position < input.length() && Character.isLetterOrDigit(input.charAt(position))) {
            position++;
        }
        return input.substring(startIndex, position);
    }
}
