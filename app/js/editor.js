/*
 Visual2 @ Imperial College London
 Project: A user - friendly ARM emulator in F# and Web Technologies(Github Electron & Fable Compliler)
 Module: JS/Monaco-init
 Description: Javascript code to run Monaco editor and define code highlighting regexes
 */


// Save Monaco's amd require and restore Node's require
var amdRequire = global.require;
global.require = nodeRequire;

//
// get tippy.js node module for awesome tooltips
// global.tippy = require('tippy.js');
// NB this does not package properly:
// SOLUTION: use tippy-all-min.js
// add to app/js directory
// add <script src="js/tippy.all.min.js"></script> to index.html
// (optional, but best practice) to allow update via yarn,
// copy this file from ./node_modules/tippy.js to app/js via webpack.config.js CopyWebpackPlugin
//


// todo: why does this work when tippy.js does not?
var path = require('path');

function uriFromPath(_path) {
    var pathName = path.resolve(_path).replace(/\\/g, '/');
    if (pathName.length > 0 && pathName.charAt(0) !== '/') {
        pathName = '/' + pathName;
    }
    return encodeURI('file://' + pathName);
}
amdRequire.config({
                  //baseUrl: uriFromPath(path.join(__dirname, '../node_modules/monaco-editor/min'))
                  baseUrl: uriFromPath(path.join(__dirname, 'js'))
                  
                  });
// workaround monaco-css not understanding the environment
self.module = undefined;
// workaround monaco-typescript not understanding the environment
self.process.browser = true;
amdRequire(['vs/editor/editor.main'], function () {
           
           monaco.languages.register({
                                     id: 'arm'
                                     });
           monaco.languages.setMonarchTokensProvider('arm', {
             // Set defaultToken to invalid to see what you do not tokenize yet
             defaultToken: 'invalid',
             
             ignoreCase: true,
             
             brackets: [
                        ['{', '}', 'delimiter.curly'],
                        ['[', ']', 'delimiter.square'],
                        ['(', ')', 'delimiter.parenthesis'],
                        ['<', '>', 'delimiter.angle']
                        ],
             
             operators: [
                         '+', '-', '*',',', '='
                         ],
             
             keywords: [
                        "ADC","ADCCS","ADCEQ","ADCGE","ADCGT","ADCHI","ADCHS","ADCLE","ADCLO","ADCLS","ADCLT","ADCMI",
                        "ADCNE","ADCCC","ADCPL","ADCS","ADCSCS","ADCSEQ","ADCSGE","ADCSGT","ADCSHI","ADCSHS","ADCSLE",
                        "ADCSLO","ADCSLS","ADCSLT","ADCSMI","ADCSNE","ADCSCC","ADCSPL","ADCSVC","ADCSVS","ADCVC","ADCVS","ADD",
                        "ADDCS","ADDEQ","ADDGE","ADDGT","ADDHI","ADDHS","ADDLE","ADDLO","ADDLS","ADDLT","ADDMI","ADDNE","ADDCC",
                        "ADDPL","ADDS","ADDSCS","ADDSEQ","ADDSGE","ADDSGT","ADDSHI","ADDSHS","ADDSLE","ADDSLO","ADDSLS","ADDSLT",
                        "ADDSMI","ADDSNE","ADDSCC","ADDSPL","ADDSVC","ADDSVS","ADDVC","ADDVS","AND","ANDCS","ANDEQ","ANDGE","ANDGT",
                        "ANDHI","ANDHS","ANDLE","ANDLO","ANDLS","ANDLT","ANDMI","ANDNE","ANDCC","ANDPL","ANDS","ANDSCS","ANDSEQ",
                        "ANDSGE","ANDSGT","ANDSHI","ANDSHS","ANDSLE","ANDSLO","ANDSLS","ANDSLT","ANDSMI","ANDSNE","ANDSCC","ANDSPL",
                        "ANDSVC","ANDSVS","ANDVC","ANDVS","ASR","ASRCS","ASREQ","ASRGE","ASRGT","ASRHI","ASRHS","ASRLE","ASRLO",
                        "ASRLS","ASRLT","ASRMI","ASRNE","ASRCC","ASRPL","ASRS","ASRSCS","ASRSEQ","ASRSGE","ASRSGT","ASRSHI","ASRSHS",
                        "ASRSLE","ASRSLO","ASRSLS","ASRSLT","ASRSMI","ASRSNE","ASRSCC","ASRSPL","ASRSVC","ASRSVS","ASRVC","ASRVS",
                        "BIC","BICCS","BICEQ","BICGE","BICGT","BICHI","BICHS","BICLE","BICLO","BICLS","BICLT","BICMI","BICNE","BICCC",
                        "BICPL","BICS","BICSCS","BICSEQ","BICSGE","BICSGT","BICSHI","BICSHS","BICSLE","BICSLO","BICSLS","BICSLT",
                        "BICSMI","BICSNE","BICSCC","BICSPL","BICSVC","BICSVS","BICVC","BICVS","CMN","CMNCS","CMNEQ","CMNGE","CMNGT",
                        "CMNHI","CMNHS","CMNLE","CMNLO","CMNLS","CMNLT","CMNMI","CMNNE","CMNCC","CMNPL","CMNS","CMNSCS","CMNSEQ",
                        "CMNSGE","CMNSGT","CMNSHI","CMNSHS","CMNSLE","CMNSLO","CMNSLS","CMNSLT","CMNSMI","CMNSNE","CMNSCC","CMNSPL",
                        "CMNSVC","CMNSVS","CMNVC","CMNVS","CMP","CMPCS","CMPEQ","CMPGE","CMPGT","CMPHI","CMPHS","CMPLE","CMPLO",
                        "CMPLS","CMPLT","CMPMI","CMPNE","CMPCC","CMPPL","CMPS","CMPSCS","CMPSEQ","CMPSGE","CMPSGT","CMPSHI","CMPSHS",
                        "CMPSLE","CMPSLO","CMPSLS","CMPSLT","CMPSMI","CMPSNE","CMPSCC","CMPSPL","CMPSVC","CMPSVS","CMPVC","CMPVS",
                        "EOR","EORCS","EOREQ","EORGE","EORGT","EORHI","EORHS","EORLE","EORLO","EORLS","EORLT","EORMI","EORNE","EORCC",
                        "EORPL","EORS","EORSCS","EORSEQ","EORSGE","EORSGT","EORSHI","EORSHS","EORSLE","EORSLO","EORSLS","EORSLT",
                        "EORSMI","EORSNE","EORSCC","EORSPL","EORSVC","EORSVS","EORVC","EORVS","LSL","LSLCS","LSLEQ","LSLGE","LSLGT",
                        "LSLHI","LSLHS","LSLLE","LSLLO","LSLLS","LSLLT","LSLMI","LSLNE","LSLCC","LSLPL","LSLS","LSLSCS","LSLSEQ",
                        "LSLSGE","LSLSGT","LSLSHI","LSLSHS","LSLSLE","LSLSLO","LSLSLS","LSLSLT","LSLSMI","LSLSNE","LSLSCC","LSLSPL",
                        "LSLSVC","LSLSVS","LSLVC","LSLVS","LSR","LSRCS","LSREQ","LSRGE","LSRGT","LSRHI","LSRHS","LSRLE","LSRLO",
                        "LSRLS","LSRLT","LSRMI","LSRNE","LSRCC","LSRPL","LSRS","LSRSCS","LSRSEQ","LSRSGE","LSRSGT","LSRSHI","LSRSHS",
                        "LSRSLE","LSRSLO","LSRSLS","LSRSLT","LSRSMI","LSRSNE","LSRSCC","LSRSPL","LSRSVC","LSRSVS","LSRVC","LSRVS",
                        "MOV","MOVCS","MOVEQ","MOVGE","MOVGT","MOVHI","MOVHS","MOVLE","MOVLO","MOVLS","MOVLT","MOVMI","MOVNE","MOVCC",
                        "MOVPL","MOVS","MOVSCS","MOVSEQ","MOVSGE","MOVSGT","MOVSHI","MOVSHS","MOVSLE","MOVSLO","MOVSLS","MOVSLT",
                        "MOVSMI","MOVSNE","MOVSCC","MOVSPL","MOVSVC","MOVSVS","MOVVC","MOVVS","MVN","MVNCS","MVNEQ","MVNGE",
                        "MVNGT","MVNHI","MVNHS","MVNLE","MVNLO","MVNLS","MVNLT","MVNMI","MVNNE","MVNCC","MVNPL","MVNS","MVNSCS",
                        "MVNSEQ","MVNSGE","MVNSGT","MVNSHI","MVNSHS","MVNSLE","MVNSLO","MVNSLS","MVNSLT","MVNSMI","MVNSNE",
                        "MVNSCC","MVNSPL","MVNSVC","MVNSVS","MVNVC","MVNVS","ORR","ORRCS","ORREQ","ORRGE","ORRGT","ORRHI",
                        "ORRHS","ORRLE","ORRLO","ORRLS","ORRLT","ORRMI","ORRNE","ORRCC","ORRPL","ORRS","ORRSCS","ORRSEQ","ORRSGE",
                        "ORRSGT","ORRSHI","ORRSHS","ORRSLE","ORRSLO","ORRSLS","ORRSLT","ORRSMI","ORRSNE","ORRSCC","ORRSPL","ORRSVC",
                        "ORRSVS","ORRVC","ORRVS","ROR","RORCS","ROREQ","RORGE","RORGT","RORHI","RORHS","RORLE","RORLO","RORLS","RORLT",
                        "RORMI","RORNE","RORCC","RORPL","RORS","RORSCS","RORSEQ","RORSGE","RORSGT","RORSHI","RORSHS","RORSLE","RORSLO",
                        "RORSLS","RORSLT","RORSMI","RORSNE","RORSCC","RORSPL","RORSVC","RORSVS","RORVC","RORVS","RRX","RRXCS","RRXEQ",
                        "RRXGE","RRXGT","RRXHI","RRXHS","RRXLE","RRXLO","RRXLS","RRXLT","RRXMI","RRXNE","RRXCC","RRXPL","RRXS","RRXSCS",
                        "RRXSEQ","RRXSGE","RRXSGT","RRXSHI","RRXSHS","RRXSLE","RRXSLO","RRXSLS","RRXSLT","RRXSMI","RRXSNE","RRXSCC",
                        "RRXSPL","RRXSVC","RRXSVS","RRXVC","RRXVS","RSB","RSBCS","RSBEQ","RSBGE","RSBGT","RSBHI","RSBHS","RSBLE",
                        "RSBLO","RSBLS","RSBLT","RSBMI","RSBNE","RSBCC","RSBPL","RSBS","RSBSCS","RSBSEQ","RSBSGE","RSBSGT","RSBSHI",
                        "RSBSHS","RSBSLE","RSBSLO","RSBSLS","RSBSLT","RSBSMI","RSBSNE","RSBSCC","RSBSPL","RSBSVC","RSBSVS","RSBVC",
                        "RSBVS","RSC","RSCCS","RSCEQ","RSCGE","RSCGT","RSCHI","RSCHS","RSCLE","RSCLO","RSCLS","RSCLT","RSCMI","RSCNE",
                        "RSCCC","RSCPL","RSCS","RSCSCS","RSCSEQ","RSCSGE","RSCSGT","RSCSHI","RSCSHS","RSCSLE","RSCSLO","RSCSLS","RSCSLT",
                        "RSCSMI","RSCSNE","RSCSCC","RSCSPL","RSCSVC","RSCSVS","RSCVC","RSCVS","SBC","SBCCS","SBCEQ","SBCGE","SBCGT",
                        "SBCHI","SBCHS","SBCLE","SBCLO","SBCLS","SBCLT","SBCMI","SBCNE","SBCCC","SBCPL","SBCS","SBCSCS","SBCSEQ",
                        "SBCSGE","SBCSGT","SBCSHI","SBCSHS","SBCSLE","SBCSLO","SBCSLS","SBCSLT","SBCSMI","SBCSNE","SBCSCC","SBCSPL",
                        "SBCSVC","SBCSVS","SBCVC","SBCVS","SUB","SUBCS","SUBEQ","SUBGE","SUBGT","SUBHI","SUBHS","SUBLE","SUBLO",
                        "SUBLS","SUBLT","SUBMI","SUBNE","SUBCC","SUBPL","SUBS","SUBSCS","SUBSEQ","SUBSGE","SUBSGT","SUBSHI","SUBSHS",
                        "SUBSLE","SUBSLO","SUBSLS","SUBSLT","SUBSMI","SUBSNE","SUBSCC","SUBSPL","SUBSVC","SUBSVS","SUBVC","SUBVS",
                        "TEQ","TEQCS","TEQEQ","TEQGE","TEQGT","TEQHI","TEQHS","TEQLE","TEQLO","TEQLS","TEQLT","TEQMI","TEQNE","TEQCC",
                        "TEQPL","TEQS","TEQSCS","TEQSEQ","TEQSGE","TEQSGT","TEQSHI","TEQSHS","TEQSLE","TEQSLO","TEQSLS","TEQSLT",
                        "TEQSMI","TEQSNE","TEQSCC","TEQSPL","TEQSVC","TEQSVS","TEQVC","TEQVS","TST","TSTCS","TSTEQ","TSTGE","TSTGT",
                        "TSTHI","TSTHS","TSTLE","TSTLO","TSTLS","TSTLT","TSTMI","TSTNE","TSTCC","TSTPL","TSTS","TSTSCS","TSTSEQ",
                        "TSTSGE","TSTSGT","TSTSHI","TSTSHS","TSTSLE","TSTSLO","TSTSLS","TSTSLT","TSTSMI","TSTSNE","TSTSCC","TSTSPL",
                        "TSTSVC","TSTSVS","TSTVC","TSTVS","B","BCS","BEQ","BGE","BGT","BHI","BHS","BL","BLCS","BLE","BLEQ","BLGE",
                        "BLGT","BLHI","BLHS","BLLE","BLLO","BLLS","BLLT","BLMI","BLNE","BLCC","BLO","BLPL","BLS","BLT","BLVC","BLVS",
                        "BMI","BNE","BCC","BPL","BVC","BVS","END","ENDCS","ENDEQ","ENDGE","ENDGT","ENDHI","ENDHS","ENDLE","ENDLO",
                        "ENDLS","ENDLT","ENDMI","ENDNE","ENDCC","ENDPL","ENDVC","ENDVS","LDM","LDMCS","LDMB","LDMBCS","LDMBEQ","LDMBGE",
                        "LDMBGT","LDMBHI","LDMBHS","LDMBLE","LDMBLO","LDMBLS","LDMBLT","LDMBMI","LDMBNE","LDMBCC","LDMBPL","LDMBVC",
                        "LDMBVS","LDMDA","LDMDACS","LDMDAEQ","LDMDAGE","LDMDAGT","LDMDAHI","LDMDAHS","LDMDALE","LDMDALO","LDMDALS",
                        "LDMDALT","LDMDAMI","LDMDANE","LDMDACC","LDMDAPL","LDMDAVC","LDMDAVS","LDMDB","LDMDBCS","LDMDBEQ","LDMDBGE",
                        "LDMDBGT","LDMDBHI","LDMDBHS","LDMDBLE","LDMDBLO","LDMDBLS","LDMDBLT","LDMDBMI","LDMDBNE","LDMDBCC","LDMDBPL",
                        "LDMDBVC","LDMDBVS","LDMEA","LDMEACS","LDMEAEQ","LDMEAGE","LDMEAGT","LDMEAHI","LDMEAHS","LDMEALE","LDMEALO",
                        "LDMEALS","LDMEALT","LDMEAMI","LDMEANE","LDMEACC","LDMEAPL","LDMEAVC","LDMEAVS","LDMED","LDMEDCS","LDMEDEQ",
                        "LDMEDGE","LDMEDGT","LDMEDHI","LDMEDHS","LDMEDLE","LDMEDLO","LDMEDLS","LDMEDLT","LDMEDMI","LDMEDNE","LDMEDCC",
                        "LDMEDPL","LDMEDVC","LDMEDVS","LDMEQ","LDMFA","LDMFACS","LDMFAEQ","LDMFAGE","LDMFAGT","LDMFAHI","LDMFAHS",
                        "LDMFALE","LDMFALO","LDMFALS","LDMFALT","LDMFAMI","LDMFANE","LDMFACC","LDMFAPL","LDMFAVC","LDMFAVS","LDMFD","LDMFDCS",
                        "LDMFDEQ","LDMFDGE","LDMFDGT","LDMFDHI","LDMFDHS","LDMFDLE","LDMFDLO","LDMFDLS","LDMFDLT","LDMFDMI","LDMFDNE",
                        "LDMFDCC","LDMFDPL","LDMFDVC","LDMFDVS","LDMGE","LDMGT","LDMHI","LDMHS","LDMIA","LDMIACS","LDMIAEQ","LDMIAGE",
                        "LDMIAGT","LDMIAHI","LDMIAHS","LDMIALE","LDMIALO","LDMIALS","LDMIALT","LDMIAMI","LDMIANE","LDMIACC","LDMIAPL",
                        "LDMIAVC","LDMIAVS","LDMIB","LDMIBCS","LDMIBEQ","LDMIBGE","LDMIBGT","LDMIBHI","LDMIBHS","LDMIBLE","LDMIBLO",
                        "LDMIBLS","LDMIBLT","LDMIBMI","LDMIBNE","LDMIBCC","LDMIBPL","LDMIBVC","LDMIBVS","LDMLE","LDMLO","LDMLS","LDMLT","LDMMI",
                        "LDMNE","LDMCC","LDMPL","LDMVC","LDMVS","LDR","LDRCS","LDRB","LDRBCS","LDRBEQ","LDRBGE","LDRBGT","LDRBHI",
                        "LDRBHS","LDRBLE","LDRBLO","LDRBLS","LDRBLT","LDRBMI","LDRBNE","LDRBCC","LDRBPL","LDRBVC","LDRBVS","LDRDA",
                        "LDRDACS","LDRDAEQ","LDRDAGE","LDRDAGT","LDRDAHI","LDRDAHS","LDRDALE","LDRDALO","LDRDALS","LDRDALT","LDRDAMI",
                        "LDRDANE","LDRDACC","LDRDAPL","LDRDAVC","LDRDAVS","LDRDB","LDRDBCS","LDRDBEQ","LDRDBGE","LDRDBGT","LDRDBHI",
                        "LDRDBHS","LDRDBLE","LDRDBLO","LDRDBLS","LDRDBLT","LDRDBMI","LDRDBNE","LDRDBCC","LDRDBPL","LDRDBVC","LDRDBVS",
                        "LDREA","LDREACS","LDREAEQ","LDREAGE","LDREAGT","LDREAHI","LDREAHS","LDREALE","LDREALO","LDREALS","LDREALT",
                        "LDREAMI","LDREANE","LDREACC","LDREAPL","LDREAVC","LDREAVS","LDRED","LDREDCS","LDREDEQ","LDREDGE","LDREDGT","LDREDHI",
                        "LDREDHS","LDREDLE","LDREDLO","LDREDLS","LDREDLT","LDREDMI","LDREDNE","LDREDCC","LDREDPL","LDREDVC","LDREDVS","LDREQ",
                        "LDRFA","LDRFACS","LDRFAEQ","LDRFAGE","LDRFAGT","LDRFAHI","LDRFAHS","LDRFALE","LDRFALO","LDRFALS","LDRFALT","LDRFAMI",
                        "LDRFANE","LDRFACC","LDRFAPL","LDRFAVC","LDRFAVS","LDRFD","LDRFDCS","LDRFDEQ","LDRFDGE","LDRFDGT","LDRFDHI","LDRFDHS",
                        "LDRFDLE","LDRFDLO","LDRFDLS","LDRFDLT","LDRFDMI","LDRFDNE","LDRFDCC","LDRFDPL","LDRFDVC","LDRFDVS","LDRGE","LDRGT",
                        "LDRHI","LDRHS","LDRIA","LDRIACS","LDRIAEQ","LDRIAGE","LDRIAGT","LDRIAHI","LDRIAHS","LDRIALE","LDRIALO","LDRIALS",
                        "LDRIALT","LDRIAMI","LDRIANE","LDRIACC","LDRIAPL","LDRIAVC","LDRIAVS","LDRIB","LDRIBCS","LDRIBEQ","LDRIBGE","LDRIBGT",
                        "LDRIBHI","LDRIBHS","LDRIBLE","LDRIBLO","LDRIBLS","LDRIBLT","LDRIBMI","LDRIBNE","LDRIBCC","LDRIBPL","LDRIBVC","LDRIBVS",
                        "LDRLE","LDRLO","LDRLS","LDRLT","LDRMI","LDRNE","LDRCC","LDRPL","LDRVC","LDRVS","STM","STMCS","STMB","STMBCS","STMBEQ",
                        "STMBGE","STMBGT","STMBHI","STMBHS","STMBLE","STMBLO","STMBLS","STMBLT","STMBMI","STMBNE","STMBCC","STMBPL","STMBVC",
                        "STMBVS","STMDA","STMDACS","STMDAEQ","STMDAGE","STMDAGT","STMDAHI","STMDAHS","STMDALE","STMDALO","STMDALS","STMDALT",
                        "STMDAMI","STMDANE","STMDACC","STMDAPL","STMDAVC","STMDAVS","STMDB","STMDBCS","STMDBEQ","STMDBGE","STMDBGT","STMDBHI",
                        "STMDBHS","STMDBLE","STMDBLO","STMDBLS","STMDBLT","STMDBMI","STMDBNE","STMDBCC","STMDBPL","STMDBVC","STMDBVS","STMEA",
                        "STMEACS","STMEAEQ","STMEAGE","STMEAGT","STMEAHI","STMEAHS","STMEALE","STMEALO","STMEALS","STMEALT","STMEAMI","STMEANE",
                        "STMEACC","STMEAPL","STMEAVC","STMEAVS","STMED","STMEDCS","STMEDEQ","STMEDGE","STMEDGT","STMEDHI","STMEDHS","STMEDLE",
                        "STMEDLO","STMEDLS","STMEDLT","STMEDMI","STMEDNE","STMEDCC","STMEDPL","STMEDVC","STMEDVS","STMEQ","STMFA","STMFACS",
                        "STMFAEQ","STMFAGE","STMFAGT","STMFAHI","STMFAHS","STMFALE","STMFALO","STMFALS","STMFALT","STMFAMI","STMFANE","STMFACC",
                        "STMFAPL","STMFAVC","STMFAVS","STMFD","STMFDCS","STMFDEQ","STMFDGE","STMFDGT","STMFDHI","STMFDHS","STMFDLE","STMFDLO",
                        "STMFDLS","STMFDLT","STMFDMI","STMFDNE","STMFDCC","STMFDPL","STMFDVC","STMFDVS","STMGE","STMGT","STMHI","STMHS","STMIA",
                        "STMIACS","STMIAEQ","STMIAGE","STMIAGT","STMIAHI","STMIAHS","STMIALE","STMIALO","STMIALS","STMIALT","STMIAMI","STMIANE",
                        "STMIACC","STMIAPL","STMIAVC","STMIAVS","STMIB","STMIBCS","STMIBEQ","STMIBGE","STMIBGT","STMIBHI","STMIBHS","STMIBLE",
                        "STMIBLO","STMIBLS","STMIBLT","STMIBMI","STMIBNE","STMIBCC","STMIBPL","STMIBVC","STMIBVS","STMLE","STMLO","STMLS","STMLT",
                        "STMMI","STMNE","STMCC","STMPL","STMVC","STMVS","STR","STRCS","STRB","STRBCS","STRBEQ","STRBGE","STRBGT","STRBHI","STRBHS",
                        "STRBLE","STRBLO","STRBLS","STRBLT","STRBMI","STRBNE","STRBCC","STRBPL","STRBVC","STRBVS","STRDA","STRDACS","STRDAEQ",
                        "STRDAGE","STRDAGT","STRDAHI","STRDAHS","STRDALE","STRDALO","STRDALS","STRDALT","STRDAMI","STRDANE","STRDACC","STRDAPL",
                        "STRDAVC","STRDAVS","STRDB","STRDBCS","STRDBEQ","STRDBGE","STRDBGT","STRDBHI","STRDBHS","STRDBLE","STRDBLO","STRDBLS",
                        "STRDBLT","STRDBMI","STRDBNE","STRDBCC","STRDBPL","STRDBVC","STRDBVS","STREA","STREACS","STREAEQ","STREAGE","STREAGT","STREAHI",
                        "STREAHS","STREALE","STREALO","STREALS","STREALT","STREAMI","STREANE","STREACC","STREAPL","STREAVC","STREAVS","STRED","STREDCS",
                        "STREDEQ","STREDGE","STREDGT","STREDHI","STREDHS","STREDLE","STREDLO","STREDLS","STREDLT","STREDMI","STREDNE","STREDCC","STREDPL",
                        "STREDVC","STREDVS","STREQ","STRFA","STRFACS","STRFAEQ","STRFAGE","STRFAGT","STRFAHI","STRFAHS","STRFALE","STRFALO","STRFALS",
                        "STRFALT","STRFAMI","STRFANE","STRFACC","STRFAPL","STRFAVC","STRFAVS","STRFD","STRFDCS","STRFDEQ","STRFDGE","STRFDGT","STRFDHI",
                        "STRFDHS","STRFDLE","STRFDLO","STRFDLS","STRFDLT","STRFDMI","STRFDNE","STRFDCC","STRFDPL","STRFDVC","STRFDVS","STRGE","STRGT",
                        "STRHI","STRHS","STRIA","STRIACS","STRIAEQ","STRIAGE","STRIAGT","STRIAHI","STRIAHS","STRIALE","STRIALO","STRIALS","STRIALT",
                        "STRIAMI","STRIANE","STRIACC","STRIAPL","STRIAVC","STRIAVS","STRIB","STRIBCS","STRIBEQ","STRIBGE","STRIBGT","STRIBHI","STRIBHS",
                        "STRIBLE","STRIBLO","STRIBLS","STRIBLT","STRIBMI","STRIBNE","STRIBCC","STRIBPL","STRIBVC","STRIBVS","STRLE","STRLO","STRLS",
                        "STRLT","STRMI","STRNE","STRCC","STRPL","STRVC","STRVS","EQU","DCD","DCB","FILL","SPACE","ADR","ADREQ","ADRNE","ADRMI","ADRPL",
                        "ADRHI","ADRHS","ADRLO","ADRLS","ADRGE","ADRGT","ADRLE","ADRLT","ADRVS","ADRVC","ADRCC","ADRCS",],
             
             // we include these common regular expressions
             symbols: /[=!~?:&|+\-*\/\^%]+/,
             
             // C# style strings
             escapes: /\\(?:[abfnrtv\\"']|x[0-9A-Fa-f]{1,4}|u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8})/,
                             
                             // The main tokenizer for our languages
                             tokenizer: {
                             root: [
                                    // identifiers and keywords
                                    [/[a-z_$][\w$]*/, {
                                     cases: {
                                     '@keywords': 'keyword',
                                     '@default': 'identifier'
                                     }
                                     }],
                                    
                                    // whitespace
                                    { include: '@whitespace' },
                                    
                                    // delimiters and operators
                                    [/[{}()\[\]]/, '@brackets'],
                                    //[/[<>](?!@symbols)/, '@brackets'],
                                    [/@symbols/, {
                                     cases: {
                                     '@operators': 'symbol.operator',
                                     '@default': 'symbol.other'
                                     }
                                     }],
                                    
                                    
                                    // numbers
                                    
                                    [/#-?0[xX][0-9a-fA-F][0-9a-fA-F_]*/, 'number.hash.hex'],
                                    [/#-?0[bB][0-1][01_]*/, 'number.hash.bin'],
                                    [/#-?\d[\d_]*/, 'number.hash'],
                                    [/-?0[xX][0-9a-fA-F][0-9a-fA-F_]*/, 'number.bare.hex'],
                                    [/-?0[bB][0-1][01_]*/, 'number.bare.bin'],
                                    [/-?\d[\d_]*/, 'number.bare'],
                                    
                                    // delimiter: after number because of .\d floats
                                    [/[,.]/, 'delimiter'],
                                    
                                    // strings
                                    [/"([^"\\]|\\.)*$/, 'string.invalid'],  // non-teminated string
                             [/"/, { token: 'string.quote', bracket: '@open', next: '@string' }],
                              
                              // characters
                              [/'[^\\']'/, 'string'],
                              [/(')(@escapes)(')/, ['string', 'string.escape', 'string']],
                              [/'/, 'string.invalid'],
                               
                               // ARM comments
                               [/;(.*)/, 'comment'],
                               [/>>;(.*)/, 'comment.testpass'],
                               [/>>-(.*)/, 'comment.testerror'],
                               
                               ],
                              
                              
                              string: [
                                       [/[^\\"]+/, 'string'],
                                          [/@escapes/, 'string.escape'],
                                          [/\\./, 'string.escape.invalid'],
                                          [/"/, { token: 'string.quote', bracket: '@close', next: '@pop' }]
                                           ],
                                          
                                          whitespace: [
                                                       [/[ \t\r\n]+/, 'white'],
                                                       //        [/\/\*/, 'comment', '@comment'],
                                                       //        [/\/\/.*$/, 'comment'],
                                                       ],
                                          }
                                          });

  window.code = monaco.editor.create(document.getElementById('editor'), {
    value: [
      'mov r0, #5',
      'mov r1, r0'
    ].join('\n'),
    language: 'arm',
    theme: 'vs-light',
    renderWhitespace: 'all',
    roundedSelection: false,
    scrollBeyondLastLine: false
  });

});