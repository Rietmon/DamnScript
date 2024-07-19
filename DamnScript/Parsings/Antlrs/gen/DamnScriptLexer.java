// Generated from /Users/rietmon/Documents/Projects/DamnScript/DamnScript/Parsings/Antlrs/DamnScriptLexer.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast", "CheckReturnValue", "this-escape"})
public class DamnScriptLexer extends Lexer {
	static { RuntimeMetaData.checkVersion("4.13.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		REGION=1, KEYWORD=2, NUMBER=3, STRING=4, IF=5, ELSEIF=6, ELSE=7, FOR=8, 
		IN=9, AND=10, OR=11, NAME=12, COMMA=13, DOT=14, SEMICOLON=15, LEFT_BRACKET=16, 
		RIGHT_BRACKET=17, LEFT_PAREN=18, RIGHT_PAREN=19, MULTIPLY=20, DIVIDE=21, 
		ADD=22, SUBTRACT=23, MODULO=24, EQUALS=25, NOT_EQUALS=26, LESS_THAN=27, 
		GREATER_THAN=28, LESS_THAN_OR_EQUAL=29, GREATER_THAN_OR_EQUAL=30, WS=31;
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	private static String[] makeRuleNames() {
		return new String[] {
			"REGION", "KEYWORD", "NUMBER", "STRING", "IF", "ELSEIF", "ELSE", "FOR", 
			"IN", "AND", "OR", "NAME", "COMMA", "DOT", "SEMICOLON", "LEFT_BRACKET", 
			"RIGHT_BRACKET", "LEFT_PAREN", "RIGHT_PAREN", "MULTIPLY", "DIVIDE", "ADD", 
			"SUBTRACT", "MODULO", "EQUALS", "NOT_EQUALS", "LESS_THAN", "GREATER_THAN", 
			"LESS_THAN_OR_EQUAL", "GREATER_THAN_OR_EQUAL", "WS"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, "'region'", null, null, null, "'if'", "'elseif'", "'else'", "'for'", 
			"'in'", "'&&'", "'||'", null, "','", "'.'", "';'", "'{'", "'}'", "'('", 
			"')'", "'*'", "'/'", "'+'", "'-'", "'%'", "'=='", "'!='", "'<'", "'>'", 
			"'<='", "'>='"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, "REGION", "KEYWORD", "NUMBER", "STRING", "IF", "ELSEIF", "ELSE", 
			"FOR", "IN", "AND", "OR", "NAME", "COMMA", "DOT", "SEMICOLON", "LEFT_BRACKET", 
			"RIGHT_BRACKET", "LEFT_PAREN", "RIGHT_PAREN", "MULTIPLY", "DIVIDE", "ADD", 
			"SUBTRACT", "MODULO", "EQUALS", "NOT_EQUALS", "LESS_THAN", "GREATER_THAN", 
			"LESS_THAN_OR_EQUAL", "GREATER_THAN_OR_EQUAL", "WS"
		};
	}
	private static final String[] _SYMBOLIC_NAMES = makeSymbolicNames();
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}


	public DamnScriptLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "DamnScriptLexer.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\u0004\u0000\u001f\u00aa\u0006\uffff\uffff\u0002\u0000\u0007\u0000\u0002"+
		"\u0001\u0007\u0001\u0002\u0002\u0007\u0002\u0002\u0003\u0007\u0003\u0002"+
		"\u0004\u0007\u0004\u0002\u0005\u0007\u0005\u0002\u0006\u0007\u0006\u0002"+
		"\u0007\u0007\u0007\u0002\b\u0007\b\u0002\t\u0007\t\u0002\n\u0007\n\u0002"+
		"\u000b\u0007\u000b\u0002\f\u0007\f\u0002\r\u0007\r\u0002\u000e\u0007\u000e"+
		"\u0002\u000f\u0007\u000f\u0002\u0010\u0007\u0010\u0002\u0011\u0007\u0011"+
		"\u0002\u0012\u0007\u0012\u0002\u0013\u0007\u0013\u0002\u0014\u0007\u0014"+
		"\u0002\u0015\u0007\u0015\u0002\u0016\u0007\u0016\u0002\u0017\u0007\u0017"+
		"\u0002\u0018\u0007\u0018\u0002\u0019\u0007\u0019\u0002\u001a\u0007\u001a"+
		"\u0002\u001b\u0007\u001b\u0002\u001c\u0007\u001c\u0002\u001d\u0007\u001d"+
		"\u0002\u001e\u0007\u001e\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000"+
		"\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0001\u0001\u0001\u0001\u0001"+
		"\u0001\u0001\u0001\u0002\u0004\u0002L\b\u0002\u000b\u0002\f\u0002M\u0001"+
		"\u0003\u0001\u0003\u0005\u0003R\b\u0003\n\u0003\f\u0003U\t\u0003\u0001"+
		"\u0003\u0001\u0003\u0001\u0004\u0001\u0004\u0001\u0004\u0001\u0005\u0001"+
		"\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0001"+
		"\u0006\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0007\u0001"+
		"\u0007\u0001\u0007\u0001\u0007\u0001\b\u0001\b\u0001\b\u0001\t\u0001\t"+
		"\u0001\t\u0001\n\u0001\n\u0001\n\u0001\u000b\u0001\u000b\u0005\u000bw"+
		"\b\u000b\n\u000b\f\u000bz\t\u000b\u0001\f\u0001\f\u0001\r\u0001\r\u0001"+
		"\u000e\u0001\u000e\u0001\u000f\u0001\u000f\u0001\u0010\u0001\u0010\u0001"+
		"\u0011\u0001\u0011\u0001\u0012\u0001\u0012\u0001\u0013\u0001\u0013\u0001"+
		"\u0014\u0001\u0014\u0001\u0015\u0001\u0015\u0001\u0016\u0001\u0016\u0001"+
		"\u0017\u0001\u0017\u0001\u0018\u0001\u0018\u0001\u0018\u0001\u0019\u0001"+
		"\u0019\u0001\u0019\u0001\u001a\u0001\u001a\u0001\u001b\u0001\u001b\u0001"+
		"\u001c\u0001\u001c\u0001\u001c\u0001\u001d\u0001\u001d\u0001\u001d\u0001"+
		"\u001e\u0004\u001e\u00a5\b\u001e\u000b\u001e\f\u001e\u00a6\u0001\u001e"+
		"\u0001\u001e\u0001S\u0000\u001f\u0001\u0001\u0003\u0002\u0005\u0003\u0007"+
		"\u0004\t\u0005\u000b\u0006\r\u0007\u000f\b\u0011\t\u0013\n\u0015\u000b"+
		"\u0017\f\u0019\r\u001b\u000e\u001d\u000f\u001f\u0010!\u0011#\u0012%\u0013"+
		"\'\u0014)\u0015+\u0016-\u0017/\u00181\u00193\u001a5\u001b7\u001c9\u001d"+
		";\u001e=\u001f\u0001\u0000\u0004\u0001\u000009\u0003\u0000AZ__az\u0004"+
		"\u000009AZ__az\u0003\u0000\t\n\r\r  \u00ad\u0000\u0001\u0001\u0000\u0000"+
		"\u0000\u0000\u0003\u0001\u0000\u0000\u0000\u0000\u0005\u0001\u0000\u0000"+
		"\u0000\u0000\u0007\u0001\u0000\u0000\u0000\u0000\t\u0001\u0000\u0000\u0000"+
		"\u0000\u000b\u0001\u0000\u0000\u0000\u0000\r\u0001\u0000\u0000\u0000\u0000"+
		"\u000f\u0001\u0000\u0000\u0000\u0000\u0011\u0001\u0000\u0000\u0000\u0000"+
		"\u0013\u0001\u0000\u0000\u0000\u0000\u0015\u0001\u0000\u0000\u0000\u0000"+
		"\u0017\u0001\u0000\u0000\u0000\u0000\u0019\u0001\u0000\u0000\u0000\u0000"+
		"\u001b\u0001\u0000\u0000\u0000\u0000\u001d\u0001\u0000\u0000\u0000\u0000"+
		"\u001f\u0001\u0000\u0000\u0000\u0000!\u0001\u0000\u0000\u0000\u0000#\u0001"+
		"\u0000\u0000\u0000\u0000%\u0001\u0000\u0000\u0000\u0000\'\u0001\u0000"+
		"\u0000\u0000\u0000)\u0001\u0000\u0000\u0000\u0000+\u0001\u0000\u0000\u0000"+
		"\u0000-\u0001\u0000\u0000\u0000\u0000/\u0001\u0000\u0000\u0000\u00001"+
		"\u0001\u0000\u0000\u0000\u00003\u0001\u0000\u0000\u0000\u00005\u0001\u0000"+
		"\u0000\u0000\u00007\u0001\u0000\u0000\u0000\u00009\u0001\u0000\u0000\u0000"+
		"\u0000;\u0001\u0000\u0000\u0000\u0000=\u0001\u0000\u0000\u0000\u0001?"+
		"\u0001\u0000\u0000\u0000\u0003F\u0001\u0000\u0000\u0000\u0005K\u0001\u0000"+
		"\u0000\u0000\u0007O\u0001\u0000\u0000\u0000\tX\u0001\u0000\u0000\u0000"+
		"\u000b[\u0001\u0000\u0000\u0000\rb\u0001\u0000\u0000\u0000\u000fg\u0001"+
		"\u0000\u0000\u0000\u0011k\u0001\u0000\u0000\u0000\u0013n\u0001\u0000\u0000"+
		"\u0000\u0015q\u0001\u0000\u0000\u0000\u0017t\u0001\u0000\u0000\u0000\u0019"+
		"{\u0001\u0000\u0000\u0000\u001b}\u0001\u0000\u0000\u0000\u001d\u007f\u0001"+
		"\u0000\u0000\u0000\u001f\u0081\u0001\u0000\u0000\u0000!\u0083\u0001\u0000"+
		"\u0000\u0000#\u0085\u0001\u0000\u0000\u0000%\u0087\u0001\u0000\u0000\u0000"+
		"\'\u0089\u0001\u0000\u0000\u0000)\u008b\u0001\u0000\u0000\u0000+\u008d"+
		"\u0001\u0000\u0000\u0000-\u008f\u0001\u0000\u0000\u0000/\u0091\u0001\u0000"+
		"\u0000\u00001\u0093\u0001\u0000\u0000\u00003\u0096\u0001\u0000\u0000\u0000"+
		"5\u0099\u0001\u0000\u0000\u00007\u009b\u0001\u0000\u0000\u00009\u009d"+
		"\u0001\u0000\u0000\u0000;\u00a0\u0001\u0000\u0000\u0000=\u00a4\u0001\u0000"+
		"\u0000\u0000?@\u0005r\u0000\u0000@A\u0005e\u0000\u0000AB\u0005g\u0000"+
		"\u0000BC\u0005i\u0000\u0000CD\u0005o\u0000\u0000DE\u0005n\u0000\u0000"+
		"E\u0002\u0001\u0000\u0000\u0000FG\u0005%\u0000\u0000GH\u0003\u0017\u000b"+
		"\u0000HI\u0005%\u0000\u0000I\u0004\u0001\u0000\u0000\u0000JL\u0007\u0000"+
		"\u0000\u0000KJ\u0001\u0000\u0000\u0000LM\u0001\u0000\u0000\u0000MK\u0001"+
		"\u0000\u0000\u0000MN\u0001\u0000\u0000\u0000N\u0006\u0001\u0000\u0000"+
		"\u0000OS\u0005\"\u0000\u0000PR\t\u0000\u0000\u0000QP\u0001\u0000\u0000"+
		"\u0000RU\u0001\u0000\u0000\u0000ST\u0001\u0000\u0000\u0000SQ\u0001\u0000"+
		"\u0000\u0000TV\u0001\u0000\u0000\u0000US\u0001\u0000\u0000\u0000VW\u0005"+
		"\"\u0000\u0000W\b\u0001\u0000\u0000\u0000XY\u0005i\u0000\u0000YZ\u0005"+
		"f\u0000\u0000Z\n\u0001\u0000\u0000\u0000[\\\u0005e\u0000\u0000\\]\u0005"+
		"l\u0000\u0000]^\u0005s\u0000\u0000^_\u0005e\u0000\u0000_`\u0005i\u0000"+
		"\u0000`a\u0005f\u0000\u0000a\f\u0001\u0000\u0000\u0000bc\u0005e\u0000"+
		"\u0000cd\u0005l\u0000\u0000de\u0005s\u0000\u0000ef\u0005e\u0000\u0000"+
		"f\u000e\u0001\u0000\u0000\u0000gh\u0005f\u0000\u0000hi\u0005o\u0000\u0000"+
		"ij\u0005r\u0000\u0000j\u0010\u0001\u0000\u0000\u0000kl\u0005i\u0000\u0000"+
		"lm\u0005n\u0000\u0000m\u0012\u0001\u0000\u0000\u0000no\u0005&\u0000\u0000"+
		"op\u0005&\u0000\u0000p\u0014\u0001\u0000\u0000\u0000qr\u0005|\u0000\u0000"+
		"rs\u0005|\u0000\u0000s\u0016\u0001\u0000\u0000\u0000tx\u0007\u0001\u0000"+
		"\u0000uw\u0007\u0002\u0000\u0000vu\u0001\u0000\u0000\u0000wz\u0001\u0000"+
		"\u0000\u0000xv\u0001\u0000\u0000\u0000xy\u0001\u0000\u0000\u0000y\u0018"+
		"\u0001\u0000\u0000\u0000zx\u0001\u0000\u0000\u0000{|\u0005,\u0000\u0000"+
		"|\u001a\u0001\u0000\u0000\u0000}~\u0005.\u0000\u0000~\u001c\u0001\u0000"+
		"\u0000\u0000\u007f\u0080\u0005;\u0000\u0000\u0080\u001e\u0001\u0000\u0000"+
		"\u0000\u0081\u0082\u0005{\u0000\u0000\u0082 \u0001\u0000\u0000\u0000\u0083"+
		"\u0084\u0005}\u0000\u0000\u0084\"\u0001\u0000\u0000\u0000\u0085\u0086"+
		"\u0005(\u0000\u0000\u0086$\u0001\u0000\u0000\u0000\u0087\u0088\u0005)"+
		"\u0000\u0000\u0088&\u0001\u0000\u0000\u0000\u0089\u008a\u0005*\u0000\u0000"+
		"\u008a(\u0001\u0000\u0000\u0000\u008b\u008c\u0005/\u0000\u0000\u008c*"+
		"\u0001\u0000\u0000\u0000\u008d\u008e\u0005+\u0000\u0000\u008e,\u0001\u0000"+
		"\u0000\u0000\u008f\u0090\u0005-\u0000\u0000\u0090.\u0001\u0000\u0000\u0000"+
		"\u0091\u0092\u0005%\u0000\u0000\u00920\u0001\u0000\u0000\u0000\u0093\u0094"+
		"\u0005=\u0000\u0000\u0094\u0095\u0005=\u0000\u0000\u00952\u0001\u0000"+
		"\u0000\u0000\u0096\u0097\u0005!\u0000\u0000\u0097\u0098\u0005=\u0000\u0000"+
		"\u00984\u0001\u0000\u0000\u0000\u0099\u009a\u0005<\u0000\u0000\u009a6"+
		"\u0001\u0000\u0000\u0000\u009b\u009c\u0005>\u0000\u0000\u009c8\u0001\u0000"+
		"\u0000\u0000\u009d\u009e\u0005<\u0000\u0000\u009e\u009f\u0005=\u0000\u0000"+
		"\u009f:\u0001\u0000\u0000\u0000\u00a0\u00a1\u0005>\u0000\u0000\u00a1\u00a2"+
		"\u0005=\u0000\u0000\u00a2<\u0001\u0000\u0000\u0000\u00a3\u00a5\u0007\u0003"+
		"\u0000\u0000\u00a4\u00a3\u0001\u0000\u0000\u0000\u00a5\u00a6\u0001\u0000"+
		"\u0000\u0000\u00a6\u00a4\u0001\u0000\u0000\u0000\u00a6\u00a7\u0001\u0000"+
		"\u0000\u0000\u00a7\u00a8\u0001\u0000\u0000\u0000\u00a8\u00a9\u0006\u001e"+
		"\u0000\u0000\u00a9>\u0001\u0000\u0000\u0000\u0005\u0000MSx\u00a6\u0001"+
		"\u0006\u0000\u0000";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}