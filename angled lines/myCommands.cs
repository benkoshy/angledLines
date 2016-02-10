// (C) Copyright 2015 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(angled_lines.MyCommands))]

namespace angled_lines
{

    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public class MyCommands
    {
        // The CommandMethod attribute can be applied to any public  member 
        // function of any public class.
        // The function should take no arguments and return nothing.
        // If the method is an intance member then the enclosing class is 
        // intantiated for each document. If the member is a static member then
        // the enclosing class is NOT intantiated.
        //
        // NOTE: CommandMethod has overloads where you can provide helpid and
        // context menu.

        // Modal Command with localized name
        [CommandMethod("MyGroup", "MyCommand", "MyCommandLocal", CommandFlags.Modal)]
        public void MyCommand() // This method can have any name
        {
            // Put your command code here
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed;
            if (doc != null)
            {
                ed = doc.Editor;
                ed.WriteMessage("Hello, this is your first command.");

            }
        }

        // Modal Command with pickfirst selection
        [CommandMethod("MyGroup", "MyPickFirst", "MyPickFirstLocal", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public void MyPickFirst() // This method can have any name
        {
            PromptSelectionResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetSelection();
            if (result.Status == PromptStatus.OK)
            {
                // There are selected entities
                // Put your command using pickfirst set code here
            }
            else
            {
                // There are no selected entities
                // Put your command code here
            }
        }

        // Application Session Command with localized name
        [CommandMethod("MyGroup", "MySessionCmd", "MySessionCmdLocal", CommandFlags.Modal | CommandFlags.Session)]
        public void MySessionCmd() // This method can have any name
        {
            // Put your command code here
        }

        // LispFunction is similar to CommandMethod but it creates a lisp 
        // callable function. Many return types are supported not just string
        // or integer.
        
        [LispFunction("MyLispFunction", "MyLispFunctionLocal")]
        public int MyLispFunction(ResultBuffer args) // This method can have any name
        {
            // Put your command code here

            // Return a value to the AutoCAD Lisp Interpreter
            
            
            return 1;
            
        }


        [CommandMethod("aa")]
        public void aa()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            
            //setting the settings for the selection filter - we want only lines filtered
            TypedValue[] acTypeValAr = new TypedValue[1] 
                                {                               
                                    new TypedValue((int)DxfCode.Start , "Line")
                                };

            // instantiating the selection filter and getting the selection result
            SelectionFilter sf = new SelectionFilter(acTypeValAr);
            PromptSelectionOptions psoAngledLines = new PromptSelectionOptions();
            psoAngledLines.MessageForAdding = "Please select rough area where angled lines exist";
            PromptSelectionResult psrAngledLines = ed.GetSelection(psoAngledLines, sf);

            //if the lines are above a certain angle then put it into an objectIDCollect
            if (psrAngledLines.Status == PromptStatus.OK)
            {

                using (Transaction tr = db.TransactionManager.StartTransaction() )
                {
                    ObjectIdCollection filteredLines = new ObjectIdCollection();
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                    

                    foreach (SelectedObject so in psrAngledLines.Value)
                    {                      
                        Line ln = tr.GetObject(so.ObjectId, OpenMode.ForWrite) as Line;   
                                if (ln != null)
                                {
                                    
                                    try
                                    {                                       
                                        //if it's a small line then delete asap.
                                        if (ln.Length < 0.2) 
                                        { ln.Erase(true); }
                                        else 
                                        {
                                                    {     
                                                        if      ((ln.Angle < (((Math.PI) / 4) + ((Math.PI) / 12)) && ln.Angle > (((Math.PI) / 4) - ((Math.PI) / 12))) ||                                                                 
                                                                (ln.Angle < (((Math.PI) * 3 / 4) + ((Math.PI) / 12)) && ln.Angle > (((Math.PI) * 3 / 4) - ((Math.PI) / 12))) ||
                                                                (ln.Angle < (((Math.PI) * 5 / 4) + ((Math.PI) / 12)) && ln.Angle > (((Math.PI) * 5 / 4) - ((Math.PI) / 12))) ||
                                                                (ln.Angle < (((Math.PI) * 7 / 4) + ((Math.PI) / 12)) && ln.Angle > (((Math.PI) * 7 / 4) - ((Math.PI) / 12))))                           
                                                                // if it meets those conditions then erase  
                                                        {
                                                            filteredLines.Add(ln.ObjectId);
                                                            ln.Erase(true);
                                                        }
                                                    } //end else statement
                                         } //if erased
	                                    }
	                                    catch 
	                                    {	
		                                    doc.Editor.WriteMessage("\nLine already deleted, or perhaps some other type of error") ;
	                                    }
                                }//end if ln!= null                                
                    }//end foreach                                              
                        tr.Commit();
                }//end using transaction


                TypedValue[] acTypeValAr1 = new TypedValue[2] 
                                {                               
                                    new TypedValue((int)DxfCode.Start , "MTEXT"),
                                    new TypedValue((int)DxfCode.Text , "*"),
                                };

                SelectionFilter sf1 = new SelectionFilter(acTypeValAr1);     
           

                psoAngledLines.MessageForAdding = "Please select rough area where angled lines exist";
                psrAngledLines = ed.GetSelection(psoAngledLines, sf1);









                //TypedValue[] acTypeValAr1 = new TypedValue[4];
                
                //acTypeValAr1.SetValue(new TypedValue((int)DxfCode.Operator, "<or"), 0);
                //acTypeValAr1.SetValue(new TypedValue((int)DxfCode.Color, 5), 1);
                //acTypeValAr1.SetValue(new TypedValue((int)DxfCode.Color, 162), 2);
                //acTypeValAr1.SetValue(new TypedValue((int)DxfCode.Operator, "or>"), 3);
                
                //SelectionFilter sf1 = new SelectionFilter(acTypeValAr1);

                //PromptSelectionResult colourDelete             =   ed.SelectAll(sf1);

                //using (Transaction tr = doc.TransactionManager.StartTransaction())
                //{
                //    foreach (Entity en in colourDelete.Value)
                //    {
                //        Entity ent = tr.GetObject(en.ObjectId, OpenMode.ForWrite) as Entity;
                //        ent.Erase();
                //    } 
                //}

                //using (Transaction tr = doc.TransactionManager.StartTransaction())
                //{
                //    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //    //BlockTableRecord btr = tr.GetObject( bt[]     );
                //}
                               
                             



                    
                //ed.WriteMessage("there are {0} of the filtered lines here", filteredLines.Count);


                    
                    
          }

           
             




       }



        //double x = ln.UnitDir.X;
        // double y = ln.UnitDir.Y;
        // double tanOfLine = y / x;
        //if (
        //((tanOfLine > 0.75 ) || (tanOfLine < 1.25 )) ||
        //((tanOfLine > 0.75 ) || (tanOfLine < 1.25 )) ||
        //((tanOfLine > Math.Tan(17 * Math.PI / 24)) || (tanOfLine < Math.Tan(19 * Math.PI / 24))) ||
        //((tanOfLine > Math.Tan(29 * Math.PI / 24)) || (tanOfLine < Math.Tan(31 * Math.PI / 24))) ||
        //((tanOfLine > Math.Tan(41 * Math.PI / 24)) || (tanOfLine < Math.Tan(43 * Math.PI / 24)))
        // )    //end of IF statement

          

        



    }//end my command
}//end namespace
