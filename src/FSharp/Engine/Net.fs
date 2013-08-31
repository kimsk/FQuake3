﻿(*
Copyright (C) 2013 William F. Smith

This program is free software; you can redistribute it
and/or modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the License,
or (at your option) any later version.

This program is distributed in the hope that it will be
useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

Derivative of Quake III Arena source:
Copyright (C) 1999-2005 Id Software, Inc.
*)

// Disable native interop warnings
#nowarn "9"
#nowarn "51"

namespace Engine.Net

open System
open System.IO
open System.Net
open System.Net.Sockets
open System.Text
open System.Runtime.InteropServices
open System.Threading
open System.Diagnostics
open Microsoft.FSharp.NativeInterop
open Engine.NativeInterop

module private Native =
    [<DllImport(LibQuake3, CallingConvention = DefaultCallingConvention)>]
    extern void NET_Init ()

module Net =
    let Init () =
        Native.NET_Init ()

    /// <summary>
    /// Based on Q3: NET_IPSocket
    /// Socket
    /// </summary
    let createIPSocket (netInterface: string option) (port: int) =
        match netInterface with
        | Some x -> printfn "Opening IP socket: %s:%i" x port
        | None -> printfn "Opening IP socket: localhost:%i" port

        let socket = 
            try
                Some <| Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            with ex ->
                printfn "WARNING: IPSocket: Socket: %s" ex.InnerException.Message
                None

        match socket with
        | None -> 0
        | Some socket ->

        socket.Blocking <- false
        socket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.Broadcast, true)

        let address =
            try
                IPAddress.Parse netInterface.Value
            with ex -> IPAddress.Any

        let port =
            match port with
            | -1 -> 0
            | _ -> port

        try
            socket.Bind (IPEndPoint (address, port))
            GCHandle.Alloc (socket, GCHandleType.Pinned) |> ignore // FIXME: We are only doing this to prevent GC when passed to unmanaged.
            int socket.Handle
        with ex ->
            printfn "WARNING: IPSocket: Bind: %s" ex.Message
            socket.Close ()
            0

