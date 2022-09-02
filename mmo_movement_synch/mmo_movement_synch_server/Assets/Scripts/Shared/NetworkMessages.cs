using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;

namespace DarkRiftRPG
{
    public enum Tags
    {
        JoinGameRequest,
        JoinGameResponse,
        SpawnLocalPlayerRequest,
        SpawnLocalPlayerResponse,
        InputPayloadTag,
        StatePayloadTag,
        SpawnPlayer,
        DespawnPlayer,
        LocalPlayerID,
        Reconcile,
        MovePlayer,
        MovePlayerPayload,
        SpawnEnemy,
        EnemyMove
    }
    public struct PlayerIdSerial : IDarkRiftSerializable
    {
        public ushort ID;

        public PlayerIdSerial(ushort id)
        {
            ID = id;

        }

        public void Deserialize(DeserializeEvent e)
        {
            ID = e.Reader.ReadUInt16();

        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);

        }
    }
    public struct PlayerDespawnData : IDarkRiftSerializable
    {
        public ushort ID;


        public PlayerDespawnData(ushort id)
        {
            ID = id;

        }

        public void Deserialize(DeserializeEvent e)
        {
            ID = e.Reader.ReadUInt16();

        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);

        }
    }
    public struct PlayerSpawnData : IDarkRiftSerializable
    {
        public ushort ID;
        public Vector3 Position;

        public PlayerSpawnData(ushort id, Vector3 position)
        {
            ID = id;
            Position = position;
        }

        public void Deserialize(DeserializeEvent e)
        {
            ID = e.Reader.ReadUInt16();
            Position = e.Reader.ReadVector3();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);
            e.Writer.WriteVector3(Position);
        }
    }

    public struct EnemySpawnData : IDarkRiftSerializable
    {
        public ushort ID;
        public Vector3 Position;

        public EnemySpawnData(ushort id, Vector3 position)
        {
            ID = id;
            Position = position;
        }

        public void Deserialize(DeserializeEvent e)
        {
            ID = e.Reader.ReadUInt16();
            Position = e.Reader.ReadVector3();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);
            e.Writer.WriteVector3(Position);
        }
    }
    public struct EnemyMovement : IDarkRiftSerializable
    {
        public ushort ID;
        public Vector3 MovePosition;

        public EnemyMovement(ushort id, Vector3 position)
        {
            ID = id;
            MovePosition = position;
        }

        public void Deserialize(DeserializeEvent e)
        {
            ID = e.Reader.ReadUInt16();
            MovePosition = e.Reader.ReadVector3();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);
            e.Writer.WriteVector3(MovePosition);
        }
    }

    public struct PlayerMoveData : IDarkRiftSerializable
    {
        public ushort ID;
        public Vector3 Position;

        public PlayerMoveData(ushort id, Vector3 position)
        {
            ID = id;
            Position = position;
        }

        public void Deserialize(DeserializeEvent e)
        {
            ID = e.Reader.ReadUInt16();
            Position = e.Reader.ReadVector3();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);
            e.Writer.WriteVector3(Position);
        }
    }

    public struct InputPayload : IDarkRiftSerializable
    {
        public int tick;
        public Vector3 inputVector;

        public InputPayload(int Tick, Vector3 InputVector)
        {
            tick = Tick;
            inputVector = InputVector;
        }

        public void Deserialize(DeserializeEvent e)
        {
            tick = e.Reader.ReadInt32();
            inputVector = e.Reader.ReadVector3();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(tick);
            e.Writer.WriteVector3(inputVector);
        }
    }
    public struct InputPayloadWithID : IDarkRiftSerializable
    {
        public ushort id;
        public int tick;
        public Vector3 inputVector;

        public InputPayloadWithID(ushort ID, int Tick, Vector3 InputVector)
        {
            id = ID;
            tick = Tick;
            inputVector = InputVector;
        }

        public void Deserialize(DeserializeEvent e)
        {
            id = e.Reader.ReadUInt16();
            tick = e.Reader.ReadInt32();
            inputVector = e.Reader.ReadVector3();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(id);
            e.Writer.Write(tick);
            e.Writer.WriteVector3(inputVector);
        }
    }

    public struct StatePayload : IDarkRiftSerializable
    {
        public int tick;
        public Vector3 position;

        public StatePayload(int Tick, Vector3 Position)
        {
            tick = Tick;
            position = Position;
        }

        public void Deserialize(DeserializeEvent e)
        {
            tick = e.Reader.ReadInt32();
            position = e.Reader.ReadVector3();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(tick);
            e.Writer.WriteVector3(position);
        }
    }
    public struct JoinGameResponseData : IDarkRiftSerializable
    {
        public bool JoinGameRequestAccepted;

        public JoinGameResponseData(bool accepted)
        {
            JoinGameRequestAccepted = accepted;
        }
        public void Deserialize(DeserializeEvent e)
        {
            JoinGameRequestAccepted = e.Reader.ReadBoolean();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(JoinGameRequestAccepted);
        }
    }

    public struct SpawnLocalPlayerResponseData : IDarkRiftSerializable
    {
        public ushort ID;

        public SpawnLocalPlayerResponseData(ushort id)
        {
            ID = id;
        }

        public void Deserialize(DeserializeEvent e)
        {
            ID = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);
        }
    }
}