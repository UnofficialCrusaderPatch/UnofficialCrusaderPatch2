[33mcommit 5892dbb37be7d658af4137bbf2cd77618c4e0bae[m[33m ([m[1;36mHEAD -> [m[1;32mmaster[m[33m, [m[1;31morigin/master[m[33m, [m[1;31morigin/HEAD[m[33m)[m
Author: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
Date:   Mon Apr 12 20:35:06 2021 +0200

    fixed some checkbox behaviour, where checkboxes wouldn't be selectable, even though there was a default option plan for it. (#783)

[33mcommit 65001d408886b1f35eb368f60c2519a9cfbc0b68[m
Author: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
Date:   Mon Apr 12 20:04:54 2021 +0200

    changed default values for fireproof, housing change and ai increased attack flat amounts (#782)
    
    tested works as expected

[33mcommit 2af285bd082ad5c8475f1cd83572eaa982e0e682[m
Author: LeSpec <62205974+LeSpec@users.noreply.github.com>
Date:   Tue Apr 6 22:29:05 2021 +0200

    spearmen run change 2.0 (#762)
    
    * spearmen run change 2.0
    
    * Update Chinese.txt
    
    * Update Chinese.txt
    
    * Update Version.cs
    
    changed default ticked to false, as this is a balance change, that shouldn'T be ticked by default.
    
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit ea6cd42921aa4dee80fe7ae20f4091f7aeb8ee73[m
Author: Nikhil <12703092+patel-nikhil@users.noreply.github.com>
Date:   Tue Apr 6 15:29:18 2021 -0400

    Revise AI housing change (#766)
    
    * Revise AI housing change
    
    * Update English.txt
    
    * Update Version.cs
    
    * Update Chinese.txt
    
    * Update German.txt
    
    * Update Hungarian.txt
    
    * Update Polish.txt
    
    * Update Russian.txt
    
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit 9f07393a01ab7ef249c5ff479a894f0d2d084b6c[m
Author: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
Date:   Mon Mar 8 13:14:48 2021 +0100

    Revert "[doc] Update CI" (#767)
    
    This reverts commit 307ca425b06d716200070245c1e6a0a9602753b6.

[33mcommit 307ca425b06d716200070245c1e6a0a9602753b6[m
Author: patel-nikhil <patenikh2@gmail.com>
Date:   Tue Jan 12 22:09:02 2021 -0500

    [doc] Update CI

[33mcommit 8fe0b1e75402b7de71e796bd96335f46ae68b76b[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Wed Feb 24 15:28:47 2021 +0100

    Add seed modification possibility (#725)
    
    * Added seed modification possibility with 2 options
    
    * Fixed crash
    
    * german translation done :)
    
    * Update Version.cs
    
    changed true to false in line 3351 to get not both checkboxes ticked by default (they should NEVER be ticked together!)
    
    * Update Chinese.txt
    
    * Update English.txt
    
    * Update German.txt
    
    * Update Hungarian.txt
    
    * Update Polish.txt
    
    * Update Russian.txt
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: LordHansCapon <walrus.grease2@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
    
    This will need some more investigation to work continuously throughout one game session and not only after restarting the game.

[33mcommit ae43931da23ba07ed76fb3fbaa524b57cb6f6ca5[m
Author: Nikhil <12703092+patel-nikhil@users.noreply.github.com>
Date:   Tue Feb 23 16:01:35 2021 -0500

    Customize AI initial defence recruit time (#733)
    
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
    
    tested by Krarilotus in Normal and Extreme,
    Tested in Normal Multiplayer with Kimberly
    Works exactly like described and expected

[33mcommit 82efab8eccf42684d9f300cc8ac548908f7a9780[m
Author: Nikhil <12703092+patel-nikhil@users.noreply.github.com>
Date:   Tue Feb 23 08:39:39 2021 -0500

    Fix bug where missing entry in start troop config caused mismatch of remaining start troops (#734)

[33mcommit 34d56ade9151368ce93449b671f83a0894778da7[m
Author: J-T-de <50496721+J-T-de@users.noreply.github.com>
Date:   Mon Feb 22 00:58:57 2021 +0100

    factor out extra defense (#758)
    
    * factor out extra defense
    
    * removed legacy fix completely
    
    * removed standard options
    
    * commas removed, that made no sense (comma after last bracket in one option)
    
    * changed the text for fix defense troops option
    
    * Update German.txt
    
    * Update Chinese.txt
    
    * Update Hungarian.txt
    
    * Update Polish.txt
    
    * Update Russian.txt
    
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit 56feaef52cff210c066bc56ea011c79902697cce[m
Author: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
Date:   Mon Feb 22 00:28:27 2021 +0100

    Changing the AIC Updater and the Project to contain these new AIC nam… (#759)
    
     Changing the AIC Updater and the Project to contain these new AIC names, and work with them:
    
    Unknown000 (WallDecoration)
    Unknown040 (AiRequestDelay)
    Unknown124 (RaidRetargetDelay)
    Unknown130 (AttAssaultDelay)
    AttUnit2 (AttUnitVanguard)
    AttUnit2Max (AttUnitVanguardMax)

[33mcommit 01d702075567b4dd3260c69d89ccbb29f6292448[m
Author: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
Date:   Mon Feb 22 00:18:15 2021 +0100

    changed mximum speed to a more reasonable 10000, which is still over 9000 (#760)
    
    tested plenty of times with 5k and less. Over 5k at own risk!

[33mcommit 32f381ac886048ae6f57b448de17fc300ee68490[m
Author: J-T-de <50496721+J-T-de@users.noreply.github.com>
Date:   Sun Feb 21 22:05:49 2021 +0100

    added German Translation by Heroesflorian (#757)
    
    reviewed german translation

[33mcommit 0b1d96a3f213392eebf95fdfa8e99a074312207b[m
Author: Mark Spiridonof <41998977+Lolasik011@users.noreply.github.com>
Date:   Sun Jan 24 19:10:01 2021 +0300

    Update Russian !!! 24.01.2021
    
    Update Russian !!! 24.01.2021

[33mcommit 812131237ee1c9fee4527211ea1e24f5a31ee747[m
Merge: cc6afb4 a1a2e99
Author: Nikhil <12703092+patel-nikhil@users.noreply.github.com>
Date:   Mon Feb 8 18:42:40 2021 -0500

    Merge pull request #753 from Liegav/master
    
    Hungarian translation

[33mcommit a1a2e99b4df25706d79d6110ceaf32d5430cb4be[m
Author: Liegav <63262183+Liegav@users.noreply.github.com>
Date:   Tue Jan 26 17:07:12 2021 +0100

    Hungarian translation

[33mcommit cc6afb4179546e47fb125fa14084ae9e25321255[m
Merge: e66f82a b99533d
Author: Nikhil <12703092+patel-nikhil@users.noreply.github.com>
Date:   Thu Jan 21 09:48:31 2021 -0500

    Merge pull request #750 from patel-nikhil/unmerge
    
    Revert unfinished changes

[33mcommit b99533dc1df70e0693b40e8067376fac6183d144[m
Author: patel-nikhil <patenikh2@gmail.com>
Date:   Wed Jan 20 18:56:52 2021 -0500

    Revert Added spearmen run change #666

[33mcommit b917eb83e13c09864242577196dc1496429245c4[m
Author: patel-nikhil <patenikh2@gmail.com>
Date:   Wed Jan 20 18:52:43 2021 -0500

    Revert Allow overbuilding workers and units #639

[33mcommit 1d5166d383a77f35c41ff5ac3e82902cd20d4c76[m
Author: patel-nikhil <patenikh2@gmail.com>
Date:   Wed Jan 20 18:49:02 2021 -0500

    Revert Fix small wall placement count preview #670

[33mcommit e66f82ab21800e1a74f06736c17f86d66a164d3f[m
Author: Nikhil <12703092+patel-nikhil@users.noreply.github.com>
Date:   Mon Dec 21 21:30:09 2020 -0500

    Revert UCP window title
    
    Revert UCP window title

[33mcommit 3ca1e2233feecf5393faa0ae55bc96ad86b3ef70[m
Author: Nikhil <12703092+patel-nikhil@users.noreply.github.com>
Date:   Mon Dec 21 19:38:07 2020 -0500

    Update AIV installation behaviour (#729)
    
    * Update AIV installation behaviour
    
    * Change default action to be the safer action
    
    * Updated AIV installation to allow install without overwriting backup and make wording more clear
    
    * Minor change to version.cs :)
    
    * Custom error messaging for file already exists error

[33mcommit 414094e3166a57d44de140f1886fff60348d3e3d[m
Author: Nikhil <12703092+patel-nikhil@users.noreply.github.com>
Date:   Sat Dec 19 21:42:50 2020 -0500

    AI housing customization (#726)
    
    Tested in Crusader HD and Extreme with all 16 AI's
    
    * AI housing customization
    
    * Add localization
    
    * Update conditional check
    
    * Add option to customize AI delete housing
    
    * Adjust housing logic
    
    * Fix csproj because I manually removed a file
    
    * Change delete housing to be buffer room after stop building houses rather than fixed value
    
    * Set max values to 500 to support random use cases
    
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit a2f445ffe9f18752d00e9fbfafb6c9e196a50a13[m
Author: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
Date:   Wed Dec 16 18:26:04 2020 +0100

    bugifxes for vanilla resources and UCP bugfix troops files (#723)
    
    * bugfix for vanilla bread 150->200
    bugfix for UCP-StartingTroops-Patch -> caliph swapped numbers of ArabArcher and Firethrowers that were mixed up

[33mcommit d31134d7893db512e9f7d6193fd9a8f7d6136410[m
Author: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
Date:   Wed Dec 16 17:59:52 2020 +0100

    german translation and increased game speed cap by efficiently setting it to 100000 (#722)
    
    * speed change and translation
    
    * German translation
    
    * fixed bug with ""
    
    * remove the speed limit effectively by setting the highest speed possible to 100000
    
    * german translation finished for now
    
    * corrected some german misshaps

[33mcommit a5fed6de0dd7559bb0239a169c320fddd595518c[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Wed Dec 16 15:52:23 2020 +0100

    Added Change siege engine spawn position option. (#714)
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit 9d61ee45a771300baf90f75c2da0a5cd67a63369[m
Author: Nikhil <12703092+patel-nikhil@users.noreply.github.com>
Date:   Wed Dec 16 09:09:30 2020 -0500

    Update CI (#720)
    
    this is an obvious one

[33mcommit 7498e5278b3eb9ce11fefe43d0840adaa98f4b91[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Wed Dec 16 15:08:08 2020 +0100

    Added stop player keep rotation change (created by gynt) (#721)
    
    * Added stop player keep rotation change
    
    * playerkeep rota description update
    
    * player keep rotate desc eng update
    
    * description german player keep rota update
    
    * hung updated descr rota keep
    
    * update polish descr player keep rota
    
    * player keep rota update russian desc
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit 1e2f649a699541e57645e5909821f0cf1f1adf12[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Mon Dec 14 22:08:08 2020 +0100

    Fixed ladderman crash. (#719)
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>

[33mcommit b7eb0b9b3419719caf5b9725f816005188e3b4bf[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Sun Nov 29 22:05:07 2020 +0100

    Restore engineer arabian speech (#686)
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit bf29b67b2cc057e3350fbc5ce5ff3e3fdaa44717[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Sun Nov 29 21:44:08 2020 +0100

    Fix Lord animation freeze (#683)
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit 02a363af9d24ffa4099ba77e4021b61d78725038[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Sun Nov 29 21:33:58 2020 +0100

    Fix small wall placement count preview (thanks to gynt) (#670)
    
    * Fix small wall placement count preview
    
    * Fixed in SHC-E too.
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit 217f42c900bdebb515dbfc0ff9c089bb714fe1e6[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Sun Nov 29 19:18:32 2020 +0100

    Added apple orchard build size fix (#662)
    
    * Added apple orchard build size fix.
    
    * Moved option to Other tab and modified description.
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit b52f6489efcecfd4869d294f1b1b8b9e08f97310[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Sun Nov 29 18:59:44 2020 +0100

    Added apple farm block fix. (#684)
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit 5fb01c0d46ebf7db385ab8da97354c44c7f63b5a[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Sun Nov 29 18:14:01 2020 +0100

    Allow overbuilding workers and units. (#639)
    
    * Allow overbuilding workers and units.
    
    * Included more units.
    Added owner check.
    Removed unnecessary code.
    
    * Added iron miner, healer and hunter dog.
    
    * Added camels and shields.
    Added neutral owner check.
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit 4f6f0956da4bbe02475e95c9733e2ef1d28fc9a2[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Sun Nov 29 17:28:14 2020 +0100

    Added option to increase the tick rate of path update by 4 times (#676)
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
    
    tested in both SHC and SHC-E

[33mcommit bc52e01b1f79f64b6621b1585777577ea40d1ecc[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Sat Nov 28 17:59:18 2020 +0100

    Fix unit ladder climb bug (#659)
    
    * Added ladderclimb bugfix.
    
    * Added missing block to fix_ladderclimb.
    
    * Added new codeblock to fix crashes when units are ordered by AI.
    
    - tested extensively with AI and in editor. Pathfinding still wierd, but not part of this fix. It fixes the initial bug!
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>

[33mcommit 582c4fd5c4431ed6fa8781ae51fc7b88d8c4b0e0[m
Author: gynt <gynt@users.noreply.github.com>
Date:   Fri Nov 27 23:27:33 2020 +0100

    Created a GitHub Actions specification for Continuous Integration (#699)
    
    * create simplified github actions
    
    * upgrade msbuild version
    
    * actually make it build
    
    * init submodules and set C# version to 4.5
    
    * update net core version
    
    * set output path
    
    * adjust output path, upload artifacts
    
    * make output path dependent on config
    
    * update artifact uploading
    
    * renamed workflow, removed debug line
    
    * cleanup
    
    * added NuGet package restoring code
    
    * allow workflow dispatch building

[33mcommit 02b7b8a37e754b77e9f02a4adb575be08b80b6b6[m[33m ([m[1;31mupstream/GUI-Update[m[33m, [m[1;31morigin/GUI-Update[m[33m)[m
Author: LordHansCapon <59705872+LordHansCapon@users.noreply.github.com>
Date:   Mon Nov 16 22:43:16 2020 +0100

    Added spearmen run change. (#666)
    
    Co-authored-by: peterkohlberger <ckpengine@gmail.com>
    Co-authored-by: Krarilotus <51748815+Krarilotus@users.noreply.github.com>
    
    approved by the master of spearmen use herself: Kimberly!
