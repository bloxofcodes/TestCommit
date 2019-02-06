SELECT 	workorders.NUM_BT as 'WOID',
		workorders.TITRE_BT as 'DESCRIPTION',
		assetfamily.CODE_FAM as 'ASSETGRP',
		tech.PRENOM_INTERV as 'TECHNICIAN',
		wostat.DES_ETAT_BT as 'WOSTAT',
		wotype.DES_TYPE_TRAV as 'WOTYPE',
		SUBSTRING(convert(varchar, workorders.DATE_DEB_REEL, 106), 4, 11) as 'YEARDATE'
FROM[SAA].[dbo].[T_BT] workorders
LEFT JOIN (SELECT * FROM[SAA].[dbo].[T_LIGNE_BT] WHERE TYPE_LIGNE_BT='I') resources ON workorders.NUM_BT = resources.CLE_BT
LEFT JOIN [SAA].[dbo].[T_INTERV] tech ON resources.CLE_ELEMENT = tech.NUM_INTERV
LEFT JOIN [SAA].[dbo].[T_UI] asset ON workorders.CLE_UI = asset.NUM_UI
LEFT JOIN [SAA].[dbo].[T_FAMILLE_UI] assetfamily ON asset.CLE_FAM = assetfamily.CLE_FAM
LEFT JOIN [SAA].[dbo].[T_ETAT_BT] wostat ON workorders.CLE_ETAT_BT = wostat.CLE_ETAT_BT
LEFT JOIN [SAA].[dbo].[T_TYPE_TRAV] wotype ON workorders.CLE_TYPE_TRAV = wotype.NUM_TYPE_TRAV
WHERE workorders.DATE_DEB_REEL >= '9/1/2018 12:00:00 AM' and workorders.DATE_DEB_REEL <= '9/30/2018 11:59:59 PM'
AND wotype.DES_TYPE_TRAV  = 'Corrective'
GROUP BY workorders.NUM_BT,workorders.TITRE_BT,assetfamily.CODE_FAM,tech.PRENOM_INTERV,wostat.DES_ETAT_BT,wotype.DES_TYPE_TRAV,workorders.DATE_DEB_REEL
ORDER BY workorders.NUM_BT ASC

-- https://www.mssqltips.com/sqlservertip/1145/date-and-time-conversions-using-sql-server/